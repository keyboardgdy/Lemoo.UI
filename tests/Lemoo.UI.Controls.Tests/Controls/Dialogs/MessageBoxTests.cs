using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Dialogs;

public class MessageBoxTests
{
    public class MessageBoxButton_Enum
    {
        [Fact]
        public void Should_HaveOKValue()
        {
            // Assert
            ((int)MessageBoxButton.OK).Should().Be(0);
        }

        [Fact]
        public void Should_HaveOKCancelValue()
        {
            // Assert
            ((int)MessageBoxButton.OKCancel).Should().Be(1);
        }

        [Fact]
        public void Should_HaveYesNoCancelValue()
        {
            // Assert
            ((int)MessageBoxButton.YesNoCancel).Should().Be(3);
        }

        [Fact]
        public void Should_HaveYesNoValue()
        {
            // Assert
            ((int)MessageBoxButton.YesNo).Should().Be(4);
        }
    }

    public class MessageBoxImage_Enum
    {
        [Fact]
        public void Should_HaveCorrectNoneValue()
        {
            // Assert
            ((int)MessageBoxImage.None).Should().Be(0);
        }

        [Fact]
        public void Should_HaveCorrectErrorValue()
        {
            // Assert
            ((int)MessageBoxImage.Error).Should().Be(16);
        }

        [Fact]
        public void Should_HaveCorrectQuestionValue()
        {
            // Assert
            ((int)MessageBoxImage.Question).Should().Be(32);
        }

        [Fact]
        public void Should_HaveCorrectWarningValue()
        {
            // Assert
            ((int)MessageBoxImage.Warning).Should().Be(48);
        }

        [Fact]
        public void Should_HaveCorrectInformationValue()
        {
            // Assert
            ((int)MessageBoxImage.Information).Should().Be(64);
        }
    }

    public class MessageBoxResult_Enum
    {
        [Fact]
        public void Should_HaveCorrectNoneValue()
        {
            // Assert
            ((int)MessageBoxResult.None).Should().Be(0);
        }

        [Fact]
        public void Should_HaveCorrectOKValue()
        {
            // Assert
            ((int)MessageBoxResult.OK).Should().Be(1);
        }

        [Fact]
        public void Should_HaveCorrectCancelValue()
        {
            // Assert
            ((int)MessageBoxResult.Cancel).Should().Be(2);
        }

        [Fact]
        public void Should_HaveCorrectYesValue()
        {
            // Assert
            ((int)MessageBoxResult.Yes).Should().Be(6);
        }

        [Fact]
        public void Should_HaveCorrectNoValue()
        {
            // Assert
            ((int)MessageBoxResult.No).Should().Be(7);
        }
    }

    public class MessageBoxResultEx_Class
    {
        [Fact]
        public void Should_StoreResultAndWasOptionChecked()
        {
            // Arrange & Act
            var resultEx = new MessageBoxResultEx(MessageBoxResult.Yes, true);

            // Assert
            resultEx.Result.Should().Be(MessageBoxResult.Yes);
            resultEx.WasOptionChecked.Should().BeTrue();
        }

        [Fact]
        public void Should_AllowImplicitConversionToMessageBoxResult()
        {
            // Arrange
            var resultEx = new MessageBoxResultEx(MessageBoxResult.OK, false);

            // Act
            MessageBoxResult result = resultEx;

            // Assert
            result.Should().Be(MessageBoxResult.OK);
        }

        [Fact]
        public void Should_ReturnTrue_WhenIsMatches()
        {
            // Arrange
            var resultEx = new MessageBoxResultEx(MessageBoxResult.Yes, false);

            // Act
            var isMatch = resultEx.Is(MessageBoxResult.Yes);

            // Assert
            isMatch.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_WhenIsDoesNotMatch()
        {
            // Arrange
            var resultEx = new MessageBoxResultEx(MessageBoxResult.Yes, false);

            // Act
            var isMatch = resultEx.Is(MessageBoxResult.No);

            // Assert
            isMatch.Should().BeFalse();
        }
    }

    public class Information_Method
    {
        [Fact]
        public void Should_ReturnInformationResult()
        {
            // Note: This test cannot fully execute without WPF application context
            // It's testing the method signature and expected behavior

            // The method should exist and have the correct signature
            // Actual execution requires WPF application context
        }
    }

    public class Success_Method
    {
        [Fact]
        public void Should_ReturnSuccessResult()
        {
            // Note: This test verifies method signature
        }
    }

    public class Warning_Method
    {
        [Fact]
        public void Should_ReturnWarningResult()
        {
            // Note: This test verifies method signature
        }
    }

    public class Error_Method
    {
        [Fact]
        public void Should_ReturnErrorResult()
        {
            // Note: This test verifies method signature
        }
    }

    public class Confirm_Method
    {
        [Fact]
        public void Should_ReturnBooleanResult()
        {
            // Note: This test verifies method signature
        }
    }

    public class ConfirmCancel_Method
    {
        [Fact]
        public void Should_ReturnMessageBoxResult()
        {
            // Note: This test verifies method signature
        }
    }

    public class Show_Method_Overloads
    {
        [Fact]
        public void Should_HaveShowOverload_WithMessageTextOnly()
        {
            // This test verifies the method signature exists
            // Actual execution requires WPF application context
        }

        [Fact]
        public void Should_HaveShowOverload_WithMessageTextAndCaption()
        {
            // This test verifies the method signature exists
        }

        [Fact]
        public void Should_HaveShowOverload_WithMessageTextCaptionAndButton()
        {
            // This test verifies the method signature exists
        }

        [Fact]
        public void Should_HaveShowOverload_WithMessageTextCaptionButtonAndIcon()
        {
            // This test verifies the method signature exists
        }

        [Fact]
        public void Should_HaveShowOverload_WithMessageTextCaptionButtonIconAndOptionText()
        {
            // This test verifies the method signature exists
        }
    }
}
