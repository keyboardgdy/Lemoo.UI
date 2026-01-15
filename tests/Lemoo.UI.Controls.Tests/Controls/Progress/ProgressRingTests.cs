using System.Windows;
using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Progress;

public class ProgressRingTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.Should().NotBeNull();
            progressRing.RingThickness.Should().Be(4.0);
            progressRing.IsIndeterminate.Should().BeFalse();
            progressRing.ShowPercentage.Should().BeFalse();
        }

        [StaFact]
        public void Should_HaveDefaultWidth40()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.Width.Should().Be(40.0);
        }

        [StaFact]
        public void Should_HaveDefaultHeight40()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.Height.Should().Be(40.0);
        }
    }

    public class RingThickness_Property
    {
        [StaFact]
        public void Should_SetAndGetRingThickness()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.RingThickness = 8.0;

            // Assert
            progressRing.RingThickness.Should().Be(8.0);
        }

        [StaFact]
        public void Should_HaveDefaultRingThickness4()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.RingThickness.Should().Be(4.0);
        }

        [StaTheory]
        [InlineData(1.0)]
        [InlineData(2.0)]
        [InlineData(4.0)]
        [InlineData(6.0)]
        [InlineData(8.0)]
        [InlineData(10.0)]
        public void Should_AcceptVariousThicknessValues(double thickness)
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.RingThickness = thickness;

            // Assert
            progressRing.RingThickness.Should().Be(thickness);
        }
    }

    public class IsIndeterminate_Property
    {
        [StaFact]
        public void Should_SetAndGetIsIndeterminate()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.IsIndeterminate = true;

            // Assert
            progressRing.IsIndeterminate.Should().BeTrue();
        }

        [StaFact]
        public void Should_HaveDefaultIsIndeterminateFalse()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.IsIndeterminate.Should().BeFalse();
        }

        [StaFact]
        public void Should_UpdateVisualState_WhenIsIndeterminateChanges()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.IsIndeterminate = true;

            // Assert
            progressRing.IsIndeterminate.Should().BeTrue();
        }
    }

    public class ShowPercentage_Property
    {
        [StaFact]
        public void Should_SetAndGetShowPercentage()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.ShowPercentage = true;

            // Assert
            progressRing.ShowPercentage.Should().BeTrue();
        }

        [StaFact]
        public void Should_HaveDefaultShowPercentageFalse()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.ShowPercentage.Should().BeFalse();
        }
    }

    public class Value_Property
    {
        [StaFact]
        public void Should_InheritValueFromProgressBar()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.Value = 50.0;

            // Assert
            progressRing.Value.Should().Be(50.0);
        }

        [StaFact]
        public void Should_HaveDefaultValue0()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.Value.Should().Be(0.0);
        }
    }

    public class Maximum_Property
    {
        [StaFact]
        public void Should_InheritMaximumFromProgressBar()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.Maximum = 200.0;

            // Assert
            progressRing.Maximum.Should().Be(200.0);
        }

        [StaFact]
        public void Should_HaveDefaultMaximum100()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.Maximum.Should().Be(100.0);
        }
    }

    public class Minimum_Property
    {
        [StaFact]
        public void Should_InheritMinimumFromProgressBar()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.Minimum = 10.0;

            // Assert
            progressRing.Minimum.Should().Be(10.0);
        }

        [StaFact]
        public void Should_HaveDefaultMinimum0()
        {
            // Act
            var progressRing = new ProgressRing();

            // Assert
            progressRing.Minimum.Should().Be(0.0);
        }
    }

    public class Size
    {
        [StaFact]
        public void Should_AllowCustomWidth()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.Width = 80.0;

            // Assert
            progressRing.Width.Should().Be(80.0);
        }

        [StaFact]
        public void Should_AllowCustomHeight()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.Height = 80.0;

            // Assert
            progressRing.Height.Should().Be(80.0);
        }

        [StaFact]
        public void Should_AllowDifferentWidthAndHeight()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.Width = 60.0;
            progressRing.Height = 80.0;

            // Assert
            progressRing.Width.Should().Be(60.0);
            progressRing.Height.Should().Be(80.0);
        }
    }

    public class EdgeCases
    {
        [StaFact]
        public void Should_HandleZeroThickness()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.RingThickness = 0.0;

            // Assert
            progressRing.RingThickness.Should().Be(0.0);
        }

        [StaFact]
        public void Should_HandleVeryLargeThickness()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.RingThickness = 100.0;

            // Assert
            progressRing.RingThickness.Should().Be(100.0);
        }

        [StaFact]
        public void Should_HandleNegativeThickness()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.RingThickness = -5.0;

            // Assert
            progressRing.RingThickness.Should().Be(-5.0);
        }

        [StaFact]
        public void Should_HandleValueAtMaximum()
        {
            // Arrange
            var progressRing = new ProgressRing { Maximum = 100 };

            // Act
            progressRing.Value = 100;

            // Assert
            progressRing.Value.Should().Be(100);
        }

        [StaFact]
        public void Should_HandleValueAtMinimum()
        {
            // Arrange
            var progressRing = new ProgressRing { Minimum = 0 };

            // Act
            progressRing.Value = 0;

            // Assert
            progressRing.Value.Should().Be(0);
        }
    }

    public class VisualState
    {
        [StaFact]
        public void Should_BeInDeterminateState_WhenIsIndeterminateIsTrue()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.IsIndeterminate = true;

            // Assert
            progressRing.IsIndeterminate.Should().BeTrue();
        }

        [StaFact]
        public void Should_BeInDeterminateState_WhenIsIndeterminateIsFalse()
        {
            // Arrange
            var progressRing = new ProgressRing { IsIndeterminate = true };

            // Act
            progressRing.IsIndeterminate = false;

            // Assert
            progressRing.IsIndeterminate.Should().BeFalse();
        }

        [StaFact]
        public void Should_HandleMultipleIsIndeterminateChanges()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.IsIndeterminate = true;
            progressRing.IsIndeterminate = false;
            progressRing.IsIndeterminate = true;
            progressRing.IsIndeterminate = false;

            // Assert
            progressRing.IsIndeterminate.Should().BeFalse();
        }
    }

    public class ProgressScenarios
    {
        [StaFact]
        public void Should_ShowDeterminateProgress()
        {
            // Arrange
            var progressRing = new ProgressRing { Maximum = 100 };

            // Act
            progressRing.Value = 75;

            // Assert
            progressRing.Value.Should().Be(75);
        }

        [StaFact]
        public void Should_ShowIndeterminateProgress()
        {
            // Arrange
            var progressRing = new ProgressRing();

            // Act
            progressRing.IsIndeterminate = true;

            // Assert
            progressRing.IsIndeterminate.Should().BeTrue();
        }

        [StaFact]
        public void Should_ShowProgressWithPercentage()
        {
            // Arrange
            var progressRing = new ProgressRing
            {
                Maximum = 100,
                Value = 50,
                ShowPercentage = true
            };

            // Assert
            progressRing.ShowPercentage.Should().BeTrue();
            progressRing.Value.Should().Be(50);
        }

        [StaFact]
        public void Should_HandleProgressFrom0To100()
        {
            // Arrange
            var progressRing = new ProgressRing { Maximum = 100 };

            // Act
            for (int i = 0; i <= 100; i += 10)
            {
                progressRing.Value = i;
            }

            // Assert
            progressRing.Value.Should().Be(100);
        }
    }
}
