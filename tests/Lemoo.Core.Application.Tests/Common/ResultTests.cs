using FluentAssertions;
using Lemoo.Core.Application.Common;
using Lemoo.Core.Common.Errors;
using Xunit;

namespace Lemoo.Core.Application.Tests.Common;

public class ResultTests
{
    public class Success
    {
        [Fact]
        public void Should_ReturnSuccessfulResult()
        {
            // Act
            var result = Result.Success();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Error.Should().BeNull();
            result.Errors.Should().BeEmpty();
            result.ErrorDetail.Should().BeNull();
        }
    }

    public class Failure_String
    {
        [Fact]
        public void Should_ReturnFailureResult_WithError()
        {
            // Act
            var result = Result.Failure("Test error");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Test error");
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnFailureResult_WithMultipleErrors()
        {
            // Arrange
            var errors = new[] { "Error 1", "Error 2", "Error 3" };

            // Act
            var result = Result.Failure(errors);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().BeEquivalentTo(errors);
        }
    }

    public class Failure_ErrorDetail
    {
        [Fact]
        public void Should_ReturnFailureResult_WithErrorDetail()
        {
            // Arrange
            var errorDetail = ErrorDetail.Create(
                LemooErrors.MODULE_NOT_FOUND,
                "Module not found",
                "TaskManager module was not found");

            // Act
            var result = Result.Failure(errorDetail);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Module not found");
            result.ErrorDetail.Should().Be(errorDetail);
            result.ErrorDetail!.Code.Should().Be(LemooErrors.MODULE_NOT_FOUND);
            result.ErrorDetail.Details.Should().Be("TaskManager module was not found");
        }

        [Fact]
        public void Should_ReturnFailureResult_WithCodeAndMessage()
        {
            // Act
            var result = Result.Failure(
                LemooErrors.VALIDATION_FAILED,
                "Validation failed",
                "Required field is missing");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorDetail.Should().NotBeNull();
            result.ErrorDetail!.Code.Should().Be(LemooErrors.VALIDATION_FAILED);
            result.ErrorDetail.Message.Should().Be("Validation failed");
            result.ErrorDetail.Details.Should().Be("Required field is missing");
        }
    }

    public class OnSuccess
    {
        [Fact]
        public void Should_ExecuteAction_WhenSuccess()
        {
            // Arrange
            var result = Result.Success();
            var executed = false;

            // Act
            result.OnSuccess(() => executed = true);

            // Assert
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_NotExecuteAction_WhenFailure()
        {
            // Arrange
            var result = Result.Failure("Error");
            var executed = false;

            // Act
            result.OnSuccess(() => executed = true);

            // Assert
            executed.Should().BeFalse();
        }
    }

    public class OnFailure
    {
        [Fact]
        public void Should_ExecuteAction_WhenFailure()
        {
            // Arrange
            var result = Result.Failure("Test error");
            string? capturedError = null;

            // Act
            result.OnFailure(error => capturedError = error);

            // Assert
            capturedError.Should().Be("Test error");
        }

        [Fact]
        public void Should_NotExecuteAction_WhenSuccess()
        {
            // Arrange
            var result = Result.Success();
            var executed = false;

            // Act
            result.OnFailure(_ => executed = true);

            // Assert
            executed.Should().BeFalse();
        }
    }

    public class Match
    {
        [Fact]
        public void Should_ReturnOnSuccess_WhenSuccess()
        {
            // Arrange
            var result = Result.Success();

            // Act
            var value = result.Match(
                onSuccess: () => "success",
                onFailure: _ => "failure");

            // Assert
            value.Should().Be("success");
        }

        [Fact]
        public void Should_ReturnOnFailure_WhenFailure()
        {
            // Arrange
            var result = Result.Failure("Test error");

            // Act
            var value = result.Match(
                onSuccess: () => "success",
                onFailure: _ => "failure");

            // Assert
            value.Should().Be("failure");
        }

        [Fact]
        public void Should_PassErrorToOnFailure()
        {
            // Arrange
            var result = Result.Failure("Test error");

            // Act
            string? capturedError = null;
            result.Match(
                onSuccess: () => "success",
                onFailure: error => { capturedError = error; return "failure"; });

            // Assert
            capturedError.Should().Be("Test error");
        }
    }

    public class Generic_Success
    {
        [Fact]
        public void Should_ReturnSuccessfulResult_WithData()
        {
            // Act
            var result = Result<int>.Success(42);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Data.Should().Be(42);
        }

        [Fact]
        public void Should_ReturnSuccessfulResult_WithObject()
        {
            // Arrange
            var data = new { Name = "Test", Value = 123 };

            // Act
            var result = Result.Success(data);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(data);
        }
    }

    public class Generic_Failure
    {
        [Fact]
        public void Should_ReturnFailureResult_WithStringError()
        {
            // Act
            var result = Result<int>.Failure("Test error");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Test error");
            result.Data.Should().Be(0);
        }

        [Fact]
        public void Should_ReturnFailureResult_WithErrorDetail()
        {
            // Arrange
            var errorDetail = ErrorDetail.Create(
                LemooErrors.ENTITY_NOT_FOUND,
                "Entity not found",
                "Task with ID 123 was not found");

            // Act
            var result = Result<int>.Failure(errorDetail);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.ErrorDetail.Should().Be(errorDetail);
            result.ErrorDetail!.Code.Should().Be(LemooErrors.ENTITY_NOT_FOUND);
        }
    }

    public class Map
    {
        [Fact]
        public void Should_TransformValue_WhenSuccess()
        {
            // Arrange
            var result = Result<int>.Success(42);

            // Act
            var mapped = result.Map(x => x.ToString());

            // Assert
            mapped.IsSuccess.Should().BeTrue();
            mapped.Data.Should().Be("42");
        }

        [Fact]
        public void Should_ReturnFailure_WhenSourceIsFailure()
        {
            // Arrange
            var result = Result<int>.Failure("Test error");

            // Act
            var mapped = result.Map(x => x.ToString());

            // Assert
            mapped.IsSuccess.Should().BeFalse();
            mapped.Error.Should().Be("Test error");
        }

        [Fact]
        public void Should_ReturnFailure_WhenDataIsNull()
        {
            // Arrange
            var result = Result<string?>.Success(null);

            // Act
            var mapped = result.Map(x => x!.Length);

            // Assert
            mapped.IsSuccess.Should().BeFalse();
            mapped.Error.Should().Be("数据为空");
        }

        [Fact]
        public void Should_ReturnFailure_WhenMapperThrows()
        {
            // Arrange
            var result = Result<int>.Success(42);

            // Act
            var mapped = result.Map<int>(x => throw new InvalidOperationException("Test exception"));

            // Assert
            mapped.IsSuccess.Should().BeFalse();
            mapped.Error.Should().Contain("映射失败");
        }
    }

    public class Bind
    {
        [Fact]
        public void Should_ChainResults_WhenAllSuccess()
        {
            // Arrange
            var result1 = Result<int>.Success(10);

            // Act
            var result = result1.Bind(x => Result<int>.Success(x * 2));

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(20);
        }

        [Fact]
        public void Should_ReturnFailure_WhenSourceIsFailure()
        {
            // Arrange
            var result1 = Result<int>.Failure("Initial error");

            // Act
            var result = result1.Bind(x => Result<int>.Success(x * 2));

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Initial error");
        }

        [Fact]
        public void Should_ReturnFailure_WhenBinderReturnsFailure()
        {
            // Arrange
            var result1 = Result<int>.Success(10);

            // Act
            var result = result1.Bind(x => Result<int>.Failure("Bind error"));

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Bind error");
        }
    }

    public class BindAsync
    {
        [Fact]
        public async Task Should_ChainResultsAsync_WhenAllSuccess()
        {
            // Arrange
            var result1 = Result<int>.Success(10);

            // Act
            var result = await result1.BindAsync(async x =>
            {
                await Task.Delay(10);
                return Result<int>.Success(x * 2);
            });

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(20);
        }

        [Fact]
        public async Task Should_ReturnFailure_WhenSourceIsFailure()
        {
            // Arrange
            var result1 = Result<int>.Failure("Initial error");

            // Act
            var result = await result1.BindAsync(async x =>
            {
                await Task.Delay(10);
                return Result<int>.Success(x * 2);
            });

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Initial error");
        }
    }

    public class ValueOr
    {
        [Fact]
        public void Should_ReturnValue_WhenSuccess()
        {
            // Arrange
            var result = Result<int>.Success(42);

            // Act
            var value = result.ValueOr(0);

            // Assert
            value.Should().Be(42);
        }

        [Fact]
        public void Should_ReturnDefaultValue_WhenFailure()
        {
            // Arrange
            var result = Result<int>.Failure("Error");

            // Act
            var value = result.ValueOr(0);

            // Assert
            value.Should().Be(0);
        }

        [Fact]
        public void Should_ReturnValueFromFactory_WhenFailure()
        {
            // Arrange
            var result = Result<int>.Failure("Error");

            // Act
            var value = result.ValueOr(() => 99);

            // Assert
            value.Should().Be(99);
        }
    }

    public class Generic_Match
    {
        [Fact]
        public void Should_ReturnOnSuccess_WhenSuccess()
        {
            // Arrange
            var result = Result<int>.Success(42);

            // Act
            var value = result.Match(
                onSuccess: data => $"Success: {data}",
                onFailure: _ => "Failure");

            // Assert
            value.Should().Be("Success: 42");
        }

        [Fact]
        public void Should_ReturnOnFailure_WhenFailure()
        {
            // Arrange
            var result = Result<int>.Failure("Test error");

            // Act
            var value = result.Match(
                onSuccess: data => $"Success: {data}",
                onFailure: _ => "Failure");

            // Assert
            value.Should().Be("Failure");
        }
    }

    public class Generic_OnSuccess
    {
        [Fact]
        public void Should_ExecuteAction_WhenSuccess()
        {
            // Arrange
            var result = Result<int>.Success(42);
            int? capturedValue = null;

            // Act
            result.OnSuccess(value => capturedValue = value);

            // Assert
            capturedValue.Should().Be(42);
        }

        [Fact]
        public void Should_NotExecuteAction_WhenFailure()
        {
            // Arrange
            var result = Result<int>.Failure("Error");
            var executed = false;

            // Act
            result.OnSuccess(_ => executed = true);

            // Assert
            executed.Should().BeFalse();
        }
    }
}
