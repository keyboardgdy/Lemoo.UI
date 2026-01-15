using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Buttons;

public class DropDownButtonTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var button = new DropDownButton();

            // Assert
            button.Should().NotBeNull();
            button.IsDropDownOpen.Should().BeFalse();
            button.Placement.Should().Be(PlacementMode.Bottom);
            button.DropDownContent.Should().BeNull();
            button.PlacementTarget.Should().BeNull();
        }
    }

    public class IsDropDownOpen_Property
    {
        [StaFact]
        public void Should_SetAndGetIsDropDownOpen()
        {
            // Arrange
            var button = new DropDownButton();
            bool openedRaised = false;
            button.DropDownOpened += (s, e) => openedRaised = true;

            // Act
            button.IsDropDownOpen = true;

            // Assert
            button.IsDropDownOpen.Should().BeTrue();
            openedRaised.Should().BeTrue();
        }

        [StaFact]
        public void Should_RaiseDropDownOpened_WhenOpening()
        {
            // Arrange
            var button = new DropDownButton();
            RoutedEventArgs? args = null;
            button.DropDownOpened += (s, e) => args = e;

            // Act
            button.IsDropDownOpen = true;

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(DropDownButton.DropDownOpenedEvent);
            args.Source.Should().Be(button);
        }

        [StaFact]
        public void Should_RaiseDropDownClosed_WhenClosing()
        {
            // Arrange
            var button = new DropDownButton();
            RoutedEventArgs? args = null;
            button.DropDownClosed += (s, e) => args = e;

            // Act
            button.IsDropDownOpen = true;
            button.IsDropDownOpen = false;

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(DropDownButton.DropDownClosedEvent);
            args.Source.Should().Be(button);
        }
    }

    public class DropDownContent_Property
    {
        [StaFact]
        public void Should_SetAndGetDropDownContent()
        {
            // Arrange
            var button = new DropDownButton();
            var menu = new ContextMenu();

            // Act
            button.DropDownContent = menu;

            // Assert
            button.DropDownContent.Should().Be(menu);
        }

        [StaFact]
        public void Should_AcceptNullDropDownContent()
        {
            // Arrange
            var button = new DropDownButton();

            // Act
            button.DropDownContent = null;

            // Assert
            button.DropDownContent.Should().BeNull();
        }

        [StaFact]
        public void Should_AcceptStringAsDropDownContent()
        {
            // Arrange
            var button = new DropDownButton();

            // Act
            button.DropDownContent = "Test Content";

            // Assert
            button.DropDownContent.Should().Be("Test Content");
        }
    }

    public class Placement_Property
    {
        [StaFact]
        public void Should_SetAndGetPlacement()
        {
            // Arrange
            var button = new DropDownButton();

            // Act
            button.Placement = PlacementMode.Right;

            // Assert
            button.Placement.Should().Be(PlacementMode.Right);
        }

        [StaTheory]
        [InlineData(PlacementMode.Top)]
        [InlineData(PlacementMode.Bottom)]
        [InlineData(PlacementMode.Left)]
        [InlineData(PlacementMode.Right)]
        [InlineData(PlacementMode.Center)]
        public void Should_AcceptAllPlacementModes(PlacementMode mode)
        {
            // Arrange
            var button = new DropDownButton();

            // Act
            button.Placement = mode;

            // Assert
            button.Placement.Should().Be(mode);
        }

        [StaFact]
        public void Should_HaveDefaultPlacementBottom()
        {
            // Act
            var button = new DropDownButton();

            // Assert
            button.Placement.Should().Be(PlacementMode.Bottom);
        }
    }

    public class PlacementTarget_Property
    {
        [StaFact]
        public void Should_SetAndGetPlacementTarget()
        {
            // Arrange
            var button = new DropDownButton();
            var target = new Grid();

            // Act
            button.PlacementTarget = target;

            // Assert
            button.PlacementTarget.Should().Be(target);
        }

        [StaFact]
        public void Should_AcceptNullPlacementTarget()
        {
            // Arrange
            var button = new DropDownButton();

            // Act
            button.PlacementTarget = null;

            // Assert
            button.PlacementTarget.Should().BeNull();
        }
    }

    public class Events
    {
        [StaFact]
        public void Should_SubscribeToDropDownOpenedEvent()
        {
            // Arrange
            var button = new DropDownButton();
            int eventCount = 0;
            button.DropDownOpened += (s, e) => eventCount++;

            // Act
            button.IsDropDownOpen = true;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_SubscribeToDropDownClosedEvent()
        {
            // Arrange
            var button = new DropDownButton();
            int eventCount = 0;
            button.DropDownClosed += (s, e) => eventCount++;

            // Act
            button.IsDropDownOpen = true;
            button.IsDropDownOpen = false;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var button = new DropDownButton();
            int eventCount1 = 0;
            int eventCount2 = 0;
            button.DropDownOpened += (s, e) => eventCount1++;
            button.DropDownOpened += (s, e) => eventCount2++;

            // Act
            button.IsDropDownOpen = true;

            // Assert
            eventCount1.Should().Be(1);
            eventCount2.Should().Be(1);
        }
    }
}
