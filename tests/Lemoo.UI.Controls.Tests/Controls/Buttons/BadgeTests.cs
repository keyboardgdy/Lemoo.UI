using FluentAssertions;
using Lemoo.UI.Controls;
using System.Windows;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Buttons;

public class BadgeTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var badge = new Badge();

            // Assert
            badge.Should().NotBeNull();
            badge.BadgeShape.Should().Be(BadgeShape.Pill);
            badge.BadgePlacement.Should().Be(BadgePlacement.TopRight);
            badge.ShowZero.Should().BeFalse();
            badge.MaxValue.Should().Be(99);
        }
    }

    public class BadgeShape_Property
    {
        [StaFact]
        public void Should_SetAndGetBadgeShape()
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.BadgeShape = BadgeShape.Circle;

            // Assert
            badge.BadgeShape.Should().Be(BadgeShape.Circle);
        }

        [StaTheory]
        [InlineData(BadgeShape.Pill)]
        [InlineData(BadgeShape.Circle)]
        [InlineData(BadgeShape.Rounded)]
        [InlineData(BadgeShape.Dot)]
        public void Should_AcceptAllBadgeShapes(BadgeShape shape)
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.BadgeShape = shape;

            // Assert
            badge.BadgeShape.Should().Be(shape);
        }
    }

    public class BadgePlacement_Property
    {
        [StaFact]
        public void Should_SetAndGetBadgePlacement()
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.BadgePlacement = BadgePlacement.BottomLeft;

            // Assert
            badge.BadgePlacement.Should().Be(BadgePlacement.BottomLeft);
        }

        [StaTheory]
        [InlineData(BadgePlacement.TopLeft)]
        [InlineData(BadgePlacement.TopRight)]
        [InlineData(BadgePlacement.BottomLeft)]
        [InlineData(BadgePlacement.BottomRight)]
        [InlineData(BadgePlacement.TopCenter)]
        [InlineData(BadgePlacement.BottomCenter)]
        public void Should_AcceptAllBadgePlacements(BadgePlacement placement)
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.BadgePlacement = placement;

            // Assert
            badge.BadgePlacement.Should().Be(placement);
        }
    }

    public class CornerRadius_Property
    {
        [StaFact]
        public void Should_SetAndGetCornerRadius()
        {
            // Arrange
            var badge = new Badge();
            var expected = new CornerRadius(8);

            // Act
            badge.CornerRadius = expected;

            // Assert
            badge.CornerRadius.Should().Be(expected);
        }

        [StaFact]
        public void Should_HaveDefaultCornerRadius()
        {
            // Act
            var badge = new Badge();

            // Assert
            badge.CornerRadius.Should().Be(new CornerRadius(12));
        }
    }

    public class ShowZero_Property
    {
        [StaFact]
        public void Should_SetAndGetShowZero()
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.ShowZero = true;

            // Assert
            badge.ShowZero.Should().BeTrue();
        }

        [StaFact]
        public void Should_HaveDefaultShowZeroFalse()
        {
            // Act
            var badge = new Badge();

            // Assert
            badge.ShowZero.Should().BeFalse();
        }
    }

    public class MaxValue_Property
    {
        [StaFact]
        public void Should_SetAndGetMaxValue()
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.MaxValue = 999;

            // Assert
            badge.MaxValue.Should().Be(999);
        }

        [StaFact]
        public void Should_HaveDefaultMaxValue99()
        {
            // Act
            var badge = new Badge();

            // Assert
            badge.MaxValue.Should().Be(99);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(99)]
        [InlineData(100)]
        [InlineData(999)]
        public void Should_AcceptValidMaxValues(int maxValue)
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.MaxValue = maxValue;

            // Assert
            badge.MaxValue.Should().Be(maxValue);
        }
    }

    public class Content_Property
    {
        [StaFact]
        public void Should_SetAndGetStringContent()
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.Content = "5";

            // Assert
            badge.Content.Should().Be("5");
        }

        [StaFact]
        public void Should_SetAndGetIntContent()
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.Content = 42;

            // Assert
            badge.Content.Should().Be(42);
        }

        [StaFact]
        public void Should_AcceptNullContent()
        {
            // Arrange
            var badge = new Badge();

            // Act
            badge.Content = null;

            // Assert
            badge.Content.Should().BeNull();
        }
    }
}
