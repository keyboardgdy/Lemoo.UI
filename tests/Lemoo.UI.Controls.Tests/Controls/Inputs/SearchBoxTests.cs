using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FluentAssertions;
using Lemoo.UI.Controls;
using Moq;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Inputs;

public class SearchBoxTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var searchBox = new SearchBox();

            // Assert
            searchBox.Should().NotBeNull();
            searchBox.Text.Should().BeEmpty();
            searchBox.PlaceholderText.Should().BeEmpty();
            searchBox.SearchCommand.Should().BeNull();
            searchBox.SearchCommandParameter.Should().BeNull();
            searchBox.ClearCommand.Should().NotBeNull();
        }

        [StaFact]
        public void Should_HaveInitializedClearCommand()
        {
            // Act
            var searchBox = new SearchBox();

            // Assert
            searchBox.ClearCommand.Should().NotBeNull();
            searchBox.ClearCommand.CanExecute(null).Should().BeTrue();
        }
    }

    public class Text_Property
    {
        [StaFact]
        public void Should_SetAndGetText()
        {
            // Arrange
            var searchBox = new SearchBox();

            // Act
            searchBox.Text = "search query";

            // Assert
            searchBox.Text.Should().Be("search query");
        }

        [StaFact]
        public void Should_AcceptEmptyString()
        {
            // Arrange
            var searchBox = new SearchBox();

            // Act
            searchBox.Text = string.Empty;

            // Assert
            searchBox.Text.Should().BeEmpty();
        }

        [StaFact]
        public void Should_RaiseTextChangedEvent_WhenTextChanged()
        {
            // Arrange
            var searchBox = new SearchBox();
            TextChangedEventArgs? args = null;
            searchBox.TextChanged += (s, e) => args = e;

            // Act
            searchBox.Text = "new text";

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(SearchBox.TextChangedEvent);
        }

        [StaFact]
        public void Should_NotRaiseTextChanged_WhenTextIsSame()
        {
            // Arrange
            var searchBox = new SearchBox { Text = "test" };
            int eventCount = 0;
            searchBox.TextChanged += (s, e) => eventCount++;

            // Act
            searchBox.Text = "test";

            // Assert
            eventCount.Should().Be(0);
        }
    }

    public class PlaceholderText_Property
    {
        [StaFact]
        public void Should_SetAndGetPlaceholderText()
        {
            // Arrange
            var searchBox = new SearchBox();

            // Act
            searchBox.PlaceholderText = "Search...";

            // Assert
            searchBox.PlaceholderText.Should().Be("Search...");
        }

        [StaFact]
        public void Should_AcceptEmptyString()
        {
            // Arrange
            var searchBox = new SearchBox();

            // Act
            searchBox.PlaceholderText = string.Empty;

            // Assert
            searchBox.PlaceholderText.Should().BeEmpty();
        }

        [StaFact]
        public void Should_HaveDefaultEmptyPlaceholderText()
        {
            // Act
            var searchBox = new SearchBox();

            // Assert
            searchBox.PlaceholderText.Should().BeEmpty();
        }
    }

    public class SearchCommand_Property
    {
        [StaFact]
        public void Should_SetAndGetSearchCommand()
        {
            // Arrange
            var searchBox = new SearchBox();
            var mockCommand = new Mock<ICommand>();

            // Act
            searchBox.SearchCommand = mockCommand.Object;

            // Assert
            searchBox.SearchCommand.Should().Be(mockCommand.Object);
        }

        [StaFact]
        public void Should_AcceptNullCommand()
        {
            // Arrange
            var searchBox = new SearchBox { SearchCommand = new Mock<ICommand>().Object };

            // Act
            searchBox.SearchCommand = null;

            // Assert
            searchBox.SearchCommand.Should().BeNull();
        }
    }

    public class SearchCommandParameter_Property
    {
        [StaFact]
        public void Should_SetAndGetSearchCommandParameter()
        {
            // Arrange
            var searchBox = new SearchBox();
            var parameter = new object();

            // Act
            searchBox.SearchCommandParameter = parameter;

            // Assert
            searchBox.SearchCommandParameter.Should().Be(parameter);
        }

        [StaFact]
        public void Should_AcceptNullParameter()
        {
            // Arrange
            var searchBox = new SearchBox { SearchCommandParameter = new object() };

            // Act
            searchBox.SearchCommandParameter = null;

            // Assert
            searchBox.SearchCommandParameter.Should().BeNull();
        }
    }

    public class ClearCommand_Property
    {
        [StaFact]
        public void Should_HaveClearCommandNotNull()
        {
            // Act
            var searchBox = new SearchBox();

            // Assert
            searchBox.ClearCommand.Should().NotBeNull();
        }

        [StaFact]
        public void Should_ClearText_WhenClearCommandExecuted()
        {
            // Arrange
            var searchBox = new SearchBox { Text = "some text" };

            // Act
            searchBox.Clear();

            // Assert
            searchBox.Text.Should().BeEmpty();
        }

        [StaFact]
        public void Should_ExecuteClearCommand()
        {
            // Arrange
            var searchBox = new SearchBox { Text = "some text" };

            // Act
            searchBox.ClearCommand.Execute(null);

            // Assert
            searchBox.Text.Should().BeEmpty();
        }
    }

    public class Clear_Method
    {
        [StaFact]
        public void Should_ClearText()
        {
            // Arrange
            var searchBox = new SearchBox { Text = "text to clear" };

            // Act
            searchBox.Clear();

            // Assert
            searchBox.Text.Should().BeEmpty();
        }

        [StaFact]
        public void Should_HandleEmptyText()
        {
            // Arrange
            var searchBox = new SearchBox { Text = string.Empty };

            // Act
            searchBox.Clear();

            // Assert
            searchBox.Text.Should().BeEmpty();
        }
    }

    public class Events
    {
        [StaFact]
        public void Should_SubscribeToTextChangedEvent()
        {
            // Arrange
            var searchBox = new SearchBox();
            int eventCount = 0;
            searchBox.TextChanged += (s, e) => eventCount++;

            // Act
            searchBox.Text = "new text";

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var searchBox = new SearchBox();
            int eventCount1 = 0;
            int eventCount2 = 0;
            searchBox.TextChanged += (s, e) => eventCount1++;
            searchBox.TextChanged += (s, e) => eventCount2++;

            // Act
            searchBox.Text = "new text";

            // Assert
            eventCount1.Should().Be(1);
            eventCount2.Should().Be(1);
        }
    }

    public class TwoWayBinding
    {
        [StaFact]
        public void Should_SupportTwoWayBinding()
        {
            // Arrange
            var searchBox = new SearchBox();

            // Act
            searchBox.Text = "test";
            var value = searchBox.Text;

            // Assert
            value.Should().Be("test");
        }
    }
}
