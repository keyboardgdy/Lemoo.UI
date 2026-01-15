using System.Windows;
using System.Windows.Input;
using FluentAssertions;
using Lemoo.UI.Controls;
using Moq;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Buttons;

public class ToggleSwitchTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var toggle = new ToggleSwitch();

            // Assert
            toggle.Should().NotBeNull();
            toggle.IsChecked.Should().BeFalse();
            toggle.OnLabel.Should().Be("开");
            toggle.OffLabel.Should().Be("关");
            toggle.Header.Should().BeNull();
            toggle.Command.Should().BeNull();
            toggle.CommandParameter.Should().BeNull();
        }
    }

    public class IsChecked_Property
    {
        [StaFact]
        public void Should_SetAndGetIsChecked()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.IsChecked = true;

            // Assert
            toggle.IsChecked.Should().BeTrue();
        }

        [StaFact]
        public void Should_RaiseCheckedEvent_WhenCheckedToTrue()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            RoutedEventArgs? args = null;
            toggle.Checked += (s, e) => args = e;

            // Act
            toggle.IsChecked = true;

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(ToggleSwitch.CheckedEvent);
            args.Source.Should().Be(toggle);
        }

        [StaFact]
        public void Should_RaiseUncheckedEvent_WhenCheckedToFalse()
        {
            // Arrange
            var toggle = new ToggleSwitch { IsChecked = true };
            RoutedEventArgs? args = null;
            toggle.Unchecked += (s, e) => args = e;

            // Act
            toggle.IsChecked = false;

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(ToggleSwitch.UncheckedEvent);
            args.Source.Should().Be(toggle);
        }

        [StaFact]
        public void Should_NotRaiseCheckedEvent_WhenAlreadyChecked()
        {
            // Arrange
            var toggle = new ToggleSwitch { IsChecked = true };
            int eventCount = 0;
            toggle.Checked += (s, e) => eventCount++;

            // Act
            toggle.IsChecked = true;

            // Assert
            eventCount.Should().Be(0);
        }

        [StaFact]
        public void Should_AcceptNullValue()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.IsChecked = null;

            // Assert
            toggle.IsChecked.Should().BeNull();
        }

        [StaFact]
        public void Should_RaiseUncheckedEvent_WhenSetToNull()
        {
            // Arrange
            var toggle = new ToggleSwitch { IsChecked = true };
            int uncheckedCount = 0;
            toggle.Unchecked += (s, e) => uncheckedCount++;

            // Act
            toggle.IsChecked = null;

            // Assert
            uncheckedCount.Should().Be(1);
        }
    }

    public class Header_Property
    {
        [StaFact]
        public void Should_SetAndGetHeader()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.Header = "Toggle Header";

            // Assert
            toggle.Header.Should().Be("Toggle Header");
        }

        [StaFact]
        public void Should_AcceptNullHeader()
        {
            // Arrange
            var toggle = new ToggleSwitch { Header = "Test" };

            // Act
            toggle.Header = null;

            // Assert
            toggle.Header.Should().BeNull();
        }

        [StaFact]
        public void Should_AcceptObjectAsHeader()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            var headerObject = new { Text = "Header" };

            // Act
            toggle.Header = headerObject;

            // Assert
            toggle.Header.Should().Be(headerObject);
        }
    }

    public class OnLabel_Property
    {
        [StaFact]
        public void Should_SetAndGetOnLabel()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.OnLabel = "ON";

            // Assert
            toggle.OnLabel.Should().Be("ON");
        }

        [StaFact]
        public void Should_HaveDefaultOnLabel()
        {
            // Act
            var toggle = new ToggleSwitch();

            // Assert
            toggle.OnLabel.Should().Be("开");
        }

        [StaFact]
        public void Should_AcceptEmptyString()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.OnLabel = string.Empty;

            // Assert
            toggle.OnLabel.Should().BeEmpty();
        }
    }

    public class OffLabel_Property
    {
        [StaFact]
        public void Should_SetAndGetOffLabel()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.OffLabel = "OFF";

            // Assert
            toggle.OffLabel.Should().Be("OFF");
        }

        [StaFact]
        public void Should_HaveDefaultOffLabel()
        {
            // Act
            var toggle = new ToggleSwitch();

            // Assert
            toggle.OffLabel.Should().Be("关");
        }

        [StaFact]
        public void Should_AcceptEmptyString()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.OffLabel = string.Empty;

            // Assert
            toggle.OffLabel.Should().BeEmpty();
        }
    }

    public class Command_Property
    {
        [StaFact]
        public void Should_SetAndGetCommand()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            var mockCommand = new Mock<ICommand>();

            // Act
            toggle.Command = mockCommand.Object;

            // Assert
            toggle.Command.Should().Be(mockCommand.Object);
        }

        [StaFact]
        public void Should_ExecuteCommand_WhenIsCheckedChanges()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(true);
            toggle.Command = mockCommand.Object;

            // Act
            toggle.IsChecked = true;

            // Assert
            mockCommand.Verify(c => c.Execute(It.IsAny<object>()), Times.Once);
        }

        [StaFact]
        public void Should_NotExecuteCommand_WhenCannotExecute()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(false);
            toggle.Command = mockCommand.Object;

            // Act
            toggle.IsChecked = true;

            // Assert
            mockCommand.Verify(c => c.Execute(It.IsAny<object>()), Times.Never);
        }

        [StaFact]
        public void Should_ExecuteCommandWithParameter()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(true);
            var parameter = new object();
            toggle.Command = mockCommand.Object;
            toggle.CommandParameter = parameter;

            // Act
            toggle.IsChecked = true;

            // Assert
            mockCommand.Verify(c => c.Execute(parameter), Times.Once);
        }
    }

    public class CommandParameter_Property
    {
        [StaFact]
        public void Should_SetAndGetCommandParameter()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            var parameter = new object();

            // Act
            toggle.CommandParameter = parameter;

            // Assert
            toggle.CommandParameter.Should().Be(parameter);
        }

        [StaFact]
        public void Should_AcceptNullParameter()
        {
            // Arrange
            var toggle = new ToggleSwitch();

            // Act
            toggle.CommandParameter = null;

            // Assert
            toggle.CommandParameter.Should().BeNull();
        }
    }

    public class Events
    {
        [StaFact]
        public void Should_SubscribeToCheckedEvent()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            int eventCount = 0;
            toggle.Checked += (s, e) => eventCount++;

            // Act
            toggle.IsChecked = true;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_SubscribeToUncheckedEvent()
        {
            // Arrange
            var toggle = new ToggleSwitch { IsChecked = true };
            int eventCount = 0;
            toggle.Unchecked += (s, e) => eventCount++;

            // Act
            toggle.IsChecked = false;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var toggle = new ToggleSwitch();
            int eventCount1 = 0;
            int eventCount2 = 0;
            toggle.Checked += (s, e) => eventCount1++;
            toggle.Checked += (s, e) => eventCount2++;

            // Act
            toggle.IsChecked = true;

            // Assert
            eventCount1.Should().Be(1);
            eventCount2.Should().Be(1);
        }
    }
}
