using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Cards;

public class CardTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var card = new Card();

            // Assert
            card.Should().NotBeNull();
            card.Header.Should().BeNull();
            card.HeaderTemplate.Should().BeNull();
            card.Footer.Should().BeNull();
            card.FooterTemplate.Should().BeNull();
        }

        [StaFact]
        public void Should_HaveDefaultCornerRadius()
        {
            // Act
            var card = new Card();

            // Assert
            card.CornerRadius.Should().Be(new CornerRadius(8));
        }

        [StaFact]
        public void Should_HaveDefaultPadding()
        {
            // Act
            var card = new Card();

            // Assert
            card.Padding.Should().Be(new Thickness(16));
        }
    }

    public class Header_Property
    {
        [StaFact]
        public void Should_SetAndGetHeader()
        {
            // Arrange
            var card = new Card();

            // Act
            card.Header = "Card Header";

            // Assert
            card.Header.Should().Be("Card Header");
        }

        [StaFact]
        public void Should_AcceptNullHeader()
        {
            // Arrange
            var card = new Card { Header = "Test" };

            // Act
            card.Header = null;

            // Assert
            card.Header.Should().BeNull();
        }

        [StaFact]
        public void Should_AcceptObjectAsHeader()
        {
            // Arrange
            var card = new Card();
            var headerObject = new { Title = "Header", Subtitle = "Subtitle" };

            // Act
            card.Header = headerObject;

            // Assert
            card.Header.Should().Be(headerObject);
        }

        [StaFact]
        public void Should_AcceptUIElementAsHeader()
        {
            // Arrange
            var card = new Card();
            var headerElement = new TextBlock { Text = "Header" };

            // Act
            card.Header = headerElement;

            // Assert
            card.Header.Should().Be(headerElement);
        }
    }

    public class HeaderTemplate_Property
    {
        [StaFact]
        public void Should_SetAndGetHeaderTemplate()
        {
            // Arrange
            var card = new Card();
            var template = new DataTemplate();

            // Act
            card.HeaderTemplate = template;

            // Assert
            card.HeaderTemplate.Should().Be(template);
        }

        [StaFact]
        public void Should_AcceptNullHeaderTemplate()
        {
            // Arrange
            var card = new Card { HeaderTemplate = new DataTemplate() };

            // Act
            card.HeaderTemplate = null;

            // Assert
            card.HeaderTemplate.Should().BeNull();
        }
    }

    public class Footer_Property
    {
        [StaFact]
        public void Should_SetAndGetFooter()
        {
            // Arrange
            var card = new Card();

            // Act
            card.Footer = "Card Footer";

            // Assert
            card.Footer.Should().Be("Card Footer");
        }

        [StaFact]
        public void Should_AcceptNullFooter()
        {
            // Arrange
            var card = new Card { Footer = "Test" };

            // Act
            card.Footer = null;

            // Assert
            card.Footer.Should().BeNull();
        }

        [StaFact]
        public void Should_AcceptObjectAsFooter()
        {
            // Arrange
            var card = new Card();
            var footerObject = new { Text = "Footer" };

            // Act
            card.Footer = footerObject;

            // Assert
            card.Footer.Should().Be(footerObject);
        }

        [StaFact]
        public void Should_AcceptUIElementAsFooter()
        {
            // Arrange
            var card = new Card();
            var footerElement = new Button { Content = "Action" };

            // Act
            card.Footer = footerElement;

            // Assert
            card.Footer.Should().Be(footerElement);
        }
    }

    public class FooterTemplate_Property
    {
        [StaFact]
        public void Should_SetAndGetFooterTemplate()
        {
            // Arrange
            var card = new Card();
            var template = new DataTemplate();

            // Act
            card.FooterTemplate = template;

            // Assert
            card.FooterTemplate.Should().Be(template);
        }

        [StaFact]
        public void Should_AcceptNullFooterTemplate()
        {
            // Arrange
            var card = new Card { FooterTemplate = new DataTemplate() };

            // Act
            card.FooterTemplate = null;

            // Assert
            card.FooterTemplate.Should().BeNull();
        }
    }

    public class CornerRadius_Property
    {
        [StaFact]
        public void Should_SetAndGetCornerRadius()
        {
            // Arrange
            var card = new Card();
            var expected = new CornerRadius(12);

            // Act
            card.CornerRadius = expected;

            // Assert
            card.CornerRadius.Should().Be(expected);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(24)]
        public void Should_AcceptVariousCornerRadiusValues(int radius)
        {
            // Arrange
            var card = new Card();
            var expected = new CornerRadius(radius);

            // Act
            card.CornerRadius = expected;

            // Assert
            card.CornerRadius.Should().Be(expected);
        }

        [StaFact]
        public void Should_AcceptUniformCornerRadius()
        {
            // Arrange
            var card = new Card();
            var expected = new CornerRadius(5, 10, 15, 20);

            // Act
            card.CornerRadius = expected;

            // Assert
            card.CornerRadius.TopLeft.Should().Be(5);
            card.CornerRadius.TopRight.Should().Be(10);
            card.CornerRadius.BottomRight.Should().Be(15);
            card.CornerRadius.BottomLeft.Should().Be(20);
        }
    }

    public class Padding_Property
    {
        [StaFact]
        public void Should_SetAndGetPadding()
        {
            // Arrange
            var card = new Card();
            var expected = new Thickness(24);

            // Act
            card.Padding = expected;

            // Assert
            card.Padding.Should().Be(expected);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(48)]
        public void Should_AcceptVariousPaddingValues(int padding)
        {
            // Arrange
            var card = new Card();
            var expected = new Thickness(padding);

            // Act
            card.Padding = expected;

            // Assert
            card.Padding.Should().Be(expected);
        }

        [StaFact]
        public void Should_AcceptNonUniformPadding()
        {
            // Arrange
            var card = new Card();
            var expected = new Thickness(8, 12, 16, 20);

            // Act
            card.Padding = expected;

            // Assert
            card.Padding.Should().Be(expected);
        }
    }

    public class Content_Property
    {
        [StaFact]
        public void Should_SetAndGetStringContent()
        {
            // Arrange
            var card = new Card();

            // Act
            card.Content = "Card content";

            // Assert
            card.Content.Should().Be("Card content");
        }

        [StaFact]
        public void Should_AcceptUIElementAsContent()
        {
            // Arrange
            var card = new Card();
            var content = new StackPanel();

            // Act
            card.Content = content;

            // Assert
            card.Content.Should().Be(content);
        }

        [StaFact]
        public void Should_AcceptNullContent()
        {
            // Arrange
            var card = new Card { Content = "Test" };

            // Act
            card.Content = null;

            // Assert
            card.Content.Should().BeNull();
        }
    }
}
