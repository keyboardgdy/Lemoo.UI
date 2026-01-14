using FluentAssertions;
using Lemoo.Core.Common.Errors;
using Xunit;

namespace Lemoo.Core.Application.Tests;

public class ErrorDetailTests
{
    public class Create
    {
        [Fact]
        public void Should_CreateErrorDetail_WithCodeAndMessage()
        {
            // Act
            var error = ErrorDetail.Create(
                "TEST001",
                "Test error message");

            // Assert
            error.Code.Should().Be("TEST001");
            error.Message.Should().Be("Test error message");
            error.Details.Should().BeNull();
            error.Metadata.Should().BeNull();
        }

        [Fact]
        public void Should_CreateErrorDetail_WithDetails()
        {
            // Act
            var error = ErrorDetail.Create(
                "TEST001",
                "Test error message",
                "Detailed error information");

            // Assert
            error.Code.Should().Be("TEST001");
            error.Message.Should().Be("Test error message");
            error.Details.Should().Be("Detailed error information");
        }

        [Fact]
        public void Should_CreateErrorDetail_WithMetadata()
        {
            // Arrange
            var metadata = new Dictionary<string, object>
            {
                ["key1"] = "value1",
                ["key2"] = 42
            };

            // Act
            var error = ErrorDetail.Create(
                "TEST001",
                "Test error message",
                null,
                metadata);

            // Assert
            error.Code.Should().Be("TEST001");
            error.Message.Should().Be("Test error message");
            error.Metadata.Should().BeEquivalentTo(metadata);
        }
    }

    public class WithMetadata
    {
        [Fact]
        public void Should_AddMetadata_ToExistingErrorDetail()
        {
            // Arrange
            var error = ErrorDetail.Create("TEST001", "Test error");

            // Act
            var updatedError = error.WithMetadata("key1", "value1");

            // Assert
            updatedError.Should().NotBeSameAs(error);
            updatedError.Metadata.Should().ContainKey("key1");
            updatedError.Metadata!["key1"].Should().Be("value1");
        }

        [Fact]
        public void Should_PreserveExistingMetadata_WhenAddingNew()
        {
            // Arrange
            var error = ErrorDetail.Create(
                "TEST001",
                "Test error",
                null,
                new Dictionary<string, object> { ["key1"] = "value1" });

            // Act
            var updatedError = error.WithMetadata("key2", 42);

            // Assert
            updatedError.Metadata.Should().ContainKey("key1");
            updatedError.Metadata!["key1"].Should().Be("value1");
            updatedError.Metadata["key2"].Should().Be(42);
        }
    }

    public class WithDetails
    {
        [Fact]
        public void Should_AddDetails_ToExistingErrorDetail()
        {
            // Arrange
            var error = ErrorDetail.Create("TEST001", "Test error");

            // Act
            var updatedError = error.WithDetails("Detailed information");

            // Assert
            updatedError.Should().NotBeSameAs(error);
            updatedError.Details.Should().Be("Detailed information");
        }

        [Fact]
        public void Should_ReplaceExistingDetails()
        {
            // Arrange
            var error = ErrorDetail.Create(
                "TEST001",
                "Test error",
                "Old details");

            // Act
            var updatedError = error.WithDetails("New details");

            // Assert
            updatedError.Details.Should().Be("New details");
        }
    }
}
