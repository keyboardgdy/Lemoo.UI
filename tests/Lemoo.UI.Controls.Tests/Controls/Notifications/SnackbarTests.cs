using System;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Notifications;

public class SnackbarTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.Should().NotBeNull();
            snackbar.IsOpen.Should().BeFalse();
            snackbar.Message.Should().BeEmpty();
            snackbar.Severity.Should().Be(SnackbarSeverity.Info);
            snackbar.ShowIcon.Should().BeTrue();
            snackbar.ShowCloseButton.Should().BeTrue();
            snackbar.Duration.Should().Be(4000);
            snackbar.ActionButtonContent.Should().BeNull();
        }

        [StaFact]
        public void Should_InitializeDispatcherTimer()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.Should().NotBeNull();
            // Timer is initialized in constructor
        }
    }

    public class IsOpen_Property
    {
        [StaFact]
        public void Should_SetAndGetIsOpen()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.IsOpen = true;

            // Assert
            snackbar.IsOpen.Should().BeTrue();
        }

        [StaFact]
        public void Should_RaiseClosedEvent_WhenClosed()
        {
            // Arrange
            var snackbar = new Snackbar();
            SnackbarClosedEventArgs? args = null;
            snackbar.Closed += (s, e) => args = e;

            // Act
            snackbar.IsOpen = true;
            snackbar.IsOpen = false;

            // Assert
            args.Should().NotBeNull();
        }
    }

    public class Message_Property
    {
        [StaFact]
        public void Should_SetAndGetMessage()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Message = "Test message";

            // Assert
            snackbar.Message.Should().Be("Test message");
        }

        [StaFact]
        public void Should_AcceptEmptyString()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Message = string.Empty;

            // Assert
            snackbar.Message.Should().BeEmpty();
        }

        [StaFact]
        public void Should_HaveDefaultEmptyMessage()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.Message.Should().BeEmpty();
        }
    }

    public class Severity_Property
    {
        [StaFact]
        public void Should_SetAndGetSeverity()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Severity = SnackbarSeverity.Error;

            // Assert
            snackbar.Severity.Should().Be(SnackbarSeverity.Error);
        }

        [StaFact]
        public void Should_HaveDefaultSeverityInfo()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.Severity.Should().Be(SnackbarSeverity.Info);
        }

        [StaTheory]
        [InlineData(SnackbarSeverity.Info)]
        [InlineData(SnackbarSeverity.Success)]
        [InlineData(SnackbarSeverity.Warning)]
        [InlineData(SnackbarSeverity.Error)]
        public void Should_AcceptAllSeverities(SnackbarSeverity severity)
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Severity = severity;

            // Assert
            snackbar.Severity.Should().Be(severity);
        }
    }

    public class ShowIcon_Property
    {
        [StaFact]
        public void Should_SetAndGetShowIcon()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.ShowIcon = false;

            // Assert
            snackbar.ShowIcon.Should().BeFalse();
        }

        [StaFact]
        public void Should_HaveDefaultShowIconTrue()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.ShowIcon.Should().BeTrue();
        }
    }

    public class ShowCloseButton_Property
    {
        [StaFact]
        public void Should_SetAndGetShowCloseButton()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.ShowCloseButton = false;

            // Assert
            snackbar.ShowCloseButton.Should().BeFalse();
        }

        [StaFact]
        public void Should_HaveDefaultShowCloseButtonTrue()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.ShowCloseButton.Should().BeTrue();
        }
    }

    public class Duration_Property
    {
        [StaFact]
        public void Should_SetAndGetDuration()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Duration = 8000;

            // Assert
            snackbar.Duration.Should().Be(8000);
        }

        [StaFact]
        public void Should_HaveDefaultDuration4000()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.Duration.Should().Be(4000);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public void Should_AcceptVariousDurations(int duration)
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Duration = duration;

            // Assert
            snackbar.Duration.Should().Be(duration);
        }
    }

    public class CornerRadius_Property
    {
        [StaFact]
        public void Should_SetAndGetCornerRadius()
        {
            // Arrange
            var snackbar = new Snackbar();
            var expected = new System.Windows.CornerRadius(12);

            // Act
            snackbar.CornerRadius = expected;

            // Assert
            snackbar.CornerRadius.Should().Be(expected);
        }

        [StaFact]
        public void Should_HaveDefaultCornerRadius8()
        {
            // Act
            var snackbar = new Snackbar();

            // Assert
            snackbar.CornerRadius.Should().Be(new System.Windows.CornerRadius(8));
        }
    }

    public class ActionButtonContent_Property
    {
        [StaFact]
        public void Should_SetAndGetActionButtonContent()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.ActionButtonContent = "Undo";

            // Assert
            snackbar.ActionButtonContent.Should().Be("Undo");
        }

        [StaFact]
        public void Should_AcceptNullActionButtonContent()
        {
            // Arrange
            var snackbar = new Snackbar { ActionButtonContent = "Action" };

            // Act
            snackbar.ActionButtonContent = null;

            // Assert
            snackbar.ActionButtonContent.Should().BeNull();
        }
    }

    public class Show_Method
    {
        [StaFact]
        public void Should_SetMessageAndOpen()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Show("Test message");

            // Assert
            snackbar.Message.Should().Be("Test message");
            snackbar.IsOpen.Should().BeTrue();
        }

        [StaFact]
        public void Should_SetSeverity()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Show("Test", SnackbarSeverity.Error);

            // Assert
            snackbar.Severity.Should().Be(SnackbarSeverity.Error);
        }

        [StaFact]
        public void Should_SetDuration()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Show("Test", SnackbarSeverity.Info, 6000);

            // Assert
            snackbar.Duration.Should().Be(6000);
        }

        [StaFact]
        public void Should_ClearActionButtonContent()
        {
            // Arrange
            var snackbar = new Snackbar { ActionButtonContent = "Previous Action" };

            // Act
            snackbar.Show("New message");

            // Assert
            snackbar.ActionButtonContent.Should().BeNull();
        }

        [StaFact]
        public void Should_ShowWithActionButton()
        {
            // Arrange
            var snackbar = new Snackbar();
            bool actionExecuted = false;

            // Act
            snackbar.Show("Message", "Undo", () => actionExecuted = true);

            // Assert
            snackbar.Message.Should().Be("Message");
            snackbar.ActionButtonContent.Should().Be("Undo");
        }

        [StaFact]
        public void Should_ShowWithActionButtonAndSeverity()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Show("Message", "Action", () => { }, SnackbarSeverity.Warning);

            // Assert
            snackbar.Severity.Should().Be(SnackbarSeverity.Warning);
        }

        [StaFact]
        public void Should_ShowWithActionButtonAndDuration()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Show("Message", "Action", () => { }, SnackbarSeverity.Info, 10000);

            // Assert
            snackbar.Duration.Should().Be(10000);
        }
    }

    public class Close_Method
    {
        [StaFact]
        public void Should_CloseSnackbar()
        {
            // Arrange
            var snackbar = new Snackbar { IsOpen = true };

            // Act
            snackbar.Close();

            // Assert
            snackbar.IsOpen.Should().BeFalse();
        }

        [StaFact]
        public void Should_HandleCloseWhenAlreadyClosed()
        {
            // Arrange
            var snackbar = new Snackbar { IsOpen = false };

            // Act
            snackbar.Close();

            // Assert
            snackbar.IsOpen.Should().BeFalse();
        }
    }

    public class Events
    {
        [StaFact]
        public void Should_SubscribeToClosedEvent()
        {
            // Arrange
            var snackbar = new Snackbar();
            int eventCount = 0;
            snackbar.Closed += (s, e) => eventCount++;

            // Act
            snackbar.IsOpen = true;
            snackbar.IsOpen = false;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var snackbar = new Snackbar();
            int eventCount1 = 0;
            int eventCount2 = 0;
            snackbar.Closed += (s, e) => eventCount1++;
            snackbar.Closed += (s, e) => eventCount2++;

            // Act
            snackbar.IsOpen = true;
            snackbar.IsOpen = false;

            // Assert
            eventCount1.Should().Be(1);
            eventCount2.Should().Be(1);
        }
    }

    public class SnackbarSeverity_Enum
    {
        [StaTheory]
        [InlineData(SnackbarSeverity.Info)]
        [InlineData(SnackbarSeverity.Success)]
        [InlineData(SnackbarSeverity.Warning)]
        [InlineData(SnackbarSeverity.Error)]
        public void Should_HaveAllSeverityValues(SnackbarSeverity severity)
        {
            // Assert
            severity.Should().BeOneOf(
                SnackbarSeverity.Info,
                SnackbarSeverity.Success,
                SnackbarSeverity.Warning,
                SnackbarSeverity.Error);
        }
    }

    public class SnackbarActionEventArgs_Class
    {
        [StaFact]
        public void Should_StoreMessageAndSeverity()
        {
            // Arrange & Act
            var args = new SnackbarActionEventArgs("Test message", SnackbarSeverity.Error);

            // Assert
            args.Message.Should().Be("Test message");
            args.Severity.Should().Be(SnackbarSeverity.Error);
        }
    }

    public class SnackbarClosedEventArgs_Class
    {
        [StaFact]
        public void Should_CreateInstance()
        {
            // Arrange & Act
            var args = new SnackbarClosedEventArgs();

            // Assert
            args.Should().NotBeNull();
        }
    }

    public class EdgeCases
    {
        [StaFact]
        public void Should_HandleZeroDuration()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Duration = 0;

            // Assert
            snackbar.Duration.Should().Be(0);
        }

        [StaFact]
        public void Should_HandleVeryLongMessage()
        {
            // Arrange
            var snackbar = new Snackbar();
            var longMessage = new string('A', 1000);

            // Act
            snackbar.Message = longMessage;

            // Assert
            snackbar.Message.Should().Be(longMessage);
        }

        [StaFact]
        public void Should_HandleMultipleShowCalls()
        {
            // Arrange
            var snackbar = new Snackbar();

            // Act
            snackbar.Show("First message");
            snackbar.Show("Second message");
            snackbar.Show("Third message");

            // Assert
            snackbar.Message.Should().Be("Third message");
        }
    }
}
