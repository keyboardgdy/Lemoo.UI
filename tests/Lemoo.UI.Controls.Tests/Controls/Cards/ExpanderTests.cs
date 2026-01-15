using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Cards;

public class ExpanderTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var expander = new Expander();

            // Assert
            expander.Should().NotBeNull();
            expander.IsExpanded.Should().BeFalse();
            expander.ExpandDirection.Should().Be(ExpandDirection.Down);
            expander.Header.Should().BeNull();
        }
    }

    public class IsExpanded_Property
    {
        [StaFact]
        public void Should_SetAndGetIsExpanded()
        {
            // Arrange
            var expander = new Expander();

            // Act
            expander.IsExpanded = true;

            // Assert
            expander.IsExpanded.Should().BeTrue();
        }

        [StaFact]
        public void Should_RaiseExpandedEvent_WhenExpanded()
        {
            // Arrange
            var expander = new Expander();
            RoutedEventArgs? args = null;
            expander.Expanded += (s, e) => args = e;

            // Act
            expander.IsExpanded = true;

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(Expander.ExpandedEvent);
            args.Source.Should().Be(expander);
        }

        [StaFact]
        public void Should_RaiseCollapsedEvent_WhenCollapsed()
        {
            // Arrange
            var expander = new Expander { IsExpanded = true };
            RoutedEventArgs? args = null;
            expander.Collapsed += (s, e) => args = e;

            // Act
            expander.IsExpanded = false;

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(Expander.CollapsedEvent);
            args.Source.Should().Be(expander);
        }

        [StaFact]
        public void Should_NotRaiseExpandedEvent_WhenAlreadyExpanded()
        {
            // Arrange
            var expander = new Expander { IsExpanded = true };
            int eventCount = 0;
            expander.Expanded += (s, e) => eventCount++;

            // Act
            expander.IsExpanded = true;

            // Assert
            eventCount.Should().Be(0);
        }

        [StaFact]
        public void Should_NotRaiseCollapsedEvent_WhenAlreadyCollapsed()
        {
            // Arrange
            var expander = new Expander();
            int eventCount = 0;
            expander.Collapsed += (s, e) => eventCount++;

            // Act
            expander.IsExpanded = false;

            // Assert
            eventCount.Should().Be(0);
        }

        [StaFact]
        public void Should_RaiseMultipleEvents_WhenToggling()
        {
            // Arrange
            var expander = new Expander();
            int expandedCount = 0;
            int collapsedCount = 0;
            expander.Expanded += (s, e) => expandedCount++;
            expander.Collapsed += (s, e) => collapsedCount++;

            // Act
            expander.IsExpanded = true;
            expander.IsExpanded = false;
            expander.IsExpanded = true;
            expander.IsExpanded = false;

            // Assert
            expandedCount.Should().Be(2);
            collapsedCount.Should().Be(2);
        }
    }

    public class ExpandDirection_Property
    {
        [StaFact]
        public void Should_SetAndGetExpandDirection()
        {
            // Arrange
            var expander = new Expander();

            // Act
            expander.ExpandDirection = ExpandDirection.Right;

            // Assert
            expander.ExpandDirection.Should().Be(ExpandDirection.Right);
        }

        [StaFact]
        public void Should_HaveDefaultExpandDirectionDown()
        {
            // Act
            var expander = new Expander();

            // Assert
            expander.ExpandDirection.Should().Be(ExpandDirection.Down);
        }

        [StaTheory]
        [InlineData(ExpandDirection.Down)]
        [InlineData(ExpandDirection.Up)]
        [InlineData(ExpandDirection.Left)]
        [InlineData(ExpandDirection.Right)]
        public void Should_AcceptAllExpandDirections(ExpandDirection direction)
        {
            // Arrange
            var expander = new Expander();

            // Act
            expander.ExpandDirection = direction;

            // Assert
            expander.ExpandDirection.Should().Be(direction);
        }
    }

    public class Header_Property
    {
        [StaFact]
        public void Should_SetAndGetHeader()
        {
            // Arrange
            var expander = new Expander();

            // Act
            expander.Header = "Expander Header";

            // Assert
            expander.Header.Should().Be("Expander Header");
        }

        [StaFact]
        public void Should_AcceptNullHeader()
        {
            // Arrange
            var expander = new Expander { Header = "Test" };

            // Act
            expander.Header = null;

            // Assert
            expander.Header.Should().BeNull();
        }

        [StaFact]
        public void Should_AcceptObjectAsHeader()
        {
            // Arrange
            var expander = new Expander();
            var headerObject = new { Title = "Header", Icon = "Icon" };

            // Act
            expander.Header = headerObject;

            // Assert
            expander.Header.Should().Be(headerObject);
        }

        [StaFact]
        public void Should_AcceptUIElementAsHeader()
        {
            // Arrange
            var expander = new Expander();
            var headerElement = new System.Windows.Controls.StackPanel();

            // Act
            expander.Header = headerElement;

            // Assert
            expander.Header.Should().Be(headerElement);
        }
    }

    public class Content_Property
    {
        [StaFact]
        public void Should_SetAndGetStringContent()
        {
            // Arrange
            var expander = new Expander();

            // Act
            expander.Content = "Expander content";

            // Assert
            expander.Content.Should().Be("Expander content");
        }

        [StaFact]
        public void Should_AcceptUIElementAsContent()
        {
            // Arrange
            var expander = new Expander();
            var content = new System.Windows.Controls.Grid();

            // Act
            expander.Content = content;

            // Assert
            expander.Content.Should().Be(content);
        }

        [StaFact]
        public void Should_AcceptNullContent()
        {
            // Arrange
            var expander = new Expander { Content = "Test" };

            // Act
            expander.Content = null;

            // Assert
            expander.Content.Should().BeNull();
        }
    }

    public class Events
    {
        [StaFact]
        public void Should_SubscribeToExpandedEvent()
        {
            // Arrange
            var expander = new Expander();
            int eventCount = 0;
            expander.Expanded += (s, e) => eventCount++;

            // Act
            expander.IsExpanded = true;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_SubscribeToCollapsedEvent()
        {
            // Arrange
            var expander = new Expander { IsExpanded = true };
            int eventCount = 0;
            expander.Collapsed += (s, e) => eventCount++;

            // Act
            expander.IsExpanded = false;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var expander = new Expander();
            int eventCount1 = 0;
            int eventCount2 = 0;
            expander.Expanded += (s, e) => eventCount1++;
            expander.Expanded += (s, e) => eventCount2++;

            // Act
            expander.IsExpanded = true;

            // Assert
            eventCount1.Should().Be(1);
            eventCount2.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowUnsubscribingFromEvents()
        {
            // Arrange
            var expander = new Expander();
            int eventCount = 0;
            System.Windows.RoutedEventHandler handler = (s, e) => eventCount++;
            expander.Expanded += handler;
            expander.Expanded -= handler;

            // Act
            expander.IsExpanded = true;

            // Assert
            eventCount.Should().Be(0);
        }
    }

    public class TwoWayBinding
    {
        [StaFact]
        public void Should_SupportTwoWayBinding()
        {
            // Arrange
            var expander = new Expander();

            // Act
            expander.IsExpanded = true;
            var value = expander.IsExpanded;

            // Assert
            value.Should().BeTrue();
        }
    }
}
