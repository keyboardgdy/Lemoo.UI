using System.Windows;
using FluentAssertions;
using Lemoo.UI.Controls;
using Xunit;

namespace Lemoo.UI.Controls.Tests.Inputs;

public class NumericUpDownTests
{
    public class Constructor
    {
        [StaFact]
        public void Should_CreateInstance_WithDefaultValues()
        {
            // Act
            var nud = new NumericUpDown();

            // Assert
            nud.Should().NotBeNull();
            nud.Value.Should().Be(0.0);
            nud.Minimum.Should().Be(0.0);
            nud.Maximum.Should().Be(100.0);
            nud.Increment.Should().Be(1.0);
            nud.DecimalPlaces.Should().Be(0);
            nud.IsReadOnly.Should().BeFalse();
            nud.IncrementValueCommand.Should().NotBeNull();
            nud.DecrementValueCommand.Should().NotBeNull();
        }

        [StaFact]
        public void Should_HaveInitializedCommands()
        {
            // Act
            var nud = new NumericUpDown();

            // Assert
            nud.IncrementValueCommand.Should().NotBeNull();
            nud.DecrementValueCommand.Should().NotBeNull();
            nud.IncrementValueCommand.CanExecute(null).Should().BeTrue();
            nud.DecrementValueCommand.CanExecute(null).Should().BeTrue();
        }
    }

    public class Value_Property
    {
        [StaFact]
        public void Should_SetAndGetValue()
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.Value = 50.5;

            // Assert
            nud.Value.Should().Be(50.5);
        }

        [StaFact]
        public void Should_RaiseValueChangedEvent_WhenValueChanged()
        {
            // Arrange
            var nud = new NumericUpDown();
            RoutedEventArgs? args = null;
            nud.ValueChanged += (s, e) => args = e;

            // Act
            nud.Value = 25.0;

            // Assert
            args.Should().NotBeNull();
            args!.RoutedEvent.Should().Be(NumericUpDown.ValueChangedEvent);
        }

        [StaFact]
        public void Should_NotRaiseValueChanged_WhenValueIsSame()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50.0 };
            int eventCount = 0;
            nud.ValueChanged += (s, e) => eventCount++;

            // Act
            nud.Value = 50.0;

            // Assert
            eventCount.Should().Be(0);
        }

        [StaFact]
        public void Should_ClampValueToMinimum_WhenBelowMinimum()
        {
            // Arrange
            var nud = new NumericUpDown { Minimum = 10, Maximum = 100 };

            // Act
            nud.Value = 5;

            // Assert
            nud.Value.Should().BeGreaterOrEqualTo(10);
        }

        [StaFact]
        public void Should_ClampValueToMaximum_WhenAboveMaximum()
        {
            // Arrange
            var nud = new NumericUpDown { Minimum = 0, Maximum = 50 };

            // Act
            nud.Value = 100;

            // Assert
            nud.Value.Should().BeLessOrEqualTo(50);
        }
    }

    public class Minimum_Property
    {
        [StaFact]
        public void Should_SetAndGetMinimum()
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.Minimum = 10.0;

            // Assert
            nud.Minimum.Should().Be(10.0);
        }

        [StaFact]
        public void Should_AdjustMinimum_WhenGreaterThanMaximum()
        {
            // Arrange
            var nud = new NumericUpDown { Maximum = 50 };

            // Act
            nud.Minimum = 100;

            // Assert
            nud.Minimum.Should().BeLessOrEqualTo(nud.Maximum);
        }

        [StaFact]
        public void Should_CoerceValue_WhenMinimumChanges()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 25, Minimum = 0, Maximum = 100 };

            // Act
            nud.Minimum = 50;

            // Assert
            nud.Value.Should().BeGreaterOrEqualTo(50);
        }

        [StaFact]
        public void Should_HaveDefaultMinimum0()
        {
            // Act
            var nud = new NumericUpDown();

            // Assert
            nud.Minimum.Should().Be(0.0);
        }
    }

    public class Maximum_Property
    {
        [StaFact]
        public void Should_SetAndGetMaximum()
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.Maximum = 200.0;

            // Assert
            nud.Maximum.Should().Be(200.0);
        }

        [StaFact]
        public void Should_AdjustMaximum_WhenLessThanMinimum()
        {
            // Arrange
            var nud = new NumericUpDown { Minimum = 50 };

            // Act
            nud.Maximum = 25;

            // Assert
            nud.Maximum.Should().BeGreaterOrEqualTo(nud.Minimum);
        }

        [StaFact]
        public void Should_CoerceValue_WhenMaximumChanges()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 75, Minimum = 0, Maximum = 100 };

            // Act
            nud.Maximum = 50;

            // Assert
            nud.Value.Should().BeLessOrEqualTo(50);
        }

        [StaFact]
        public void Should_HaveDefaultMaximum100()
        {
            // Act
            var nud = new NumericUpDown();

            // Assert
            nud.Maximum.Should().Be(100.0);
        }
    }

    public class Increment_Property
    {
        [StaFact]
        public void Should_SetAndGetIncrement()
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.Increment = 5.0;

            // Assert
            nud.Increment.Should().Be(5.0);
        }

        [StaFact]
        public void Should_HaveDefaultIncrement1()
        {
            // Act
            var nud = new NumericUpDown();

            // Assert
            nud.Increment.Should().Be(1.0);
        }

        [StaTheory]
        [InlineData(0.1)]
        [InlineData(0.5)]
        [InlineData(1.0)]
        [InlineData(5.0)]
        [InlineData(10.0)]
        public void Should_AcceptVariousIncrementValues(double increment)
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.Increment = increment;

            // Assert
            nud.Increment.Should().Be(increment);
        }
    }

    public class DecimalPlaces_Property
    {
        [StaFact]
        public void Should_SetAndGetDecimalPlaces()
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.DecimalPlaces = 2;

            // Assert
            nud.DecimalPlaces.Should().Be(2);
        }

        [StaFact]
        public void Should_RoundValue_WhenDecimalPlacesChanges()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 12.345, DecimalPlaces = 0 };

            // Act
            nud.DecimalPlaces = 2;

            // Assert
            nud.Value.Should().BeApproximately(12.35, 0.01);
        }

        [StaFact]
        public void Should_HaveDefaultDecimalPlaces0()
        {
            // Act
            var nud = new NumericUpDown();

            // Assert
            nud.DecimalPlaces.Should().Be(0);
        }

        [StaTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_AcceptVariousDecimalPlaces(int decimalPlaces)
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.DecimalPlaces = decimalPlaces;

            // Assert
            nud.DecimalPlaces.Should().Be(decimalPlaces);
        }
    }

    public class IsReadOnly_Property
    {
        [StaFact]
        public void Should_SetAndGetIsReadOnly()
        {
            // Arrange
            var nud = new NumericUpDown();

            // Act
            nud.IsReadOnly = true;

            // Assert
            nud.IsReadOnly.Should().BeTrue();
        }

        [StaFact]
        public void Should_HaveDefaultIsReadOnlyFalse()
        {
            // Act
            var nud = new NumericUpDown();

            // Assert
            nud.IsReadOnly.Should().BeFalse();
        }
    }

    public class IncrementValue_Method
    {
        [StaFact]
        public void Should_IncreaseValue_WhenIncrementValueCalled()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5, Maximum = 100 };

            // Act
            nud.IncrementValue();

            // Assert
            nud.Value.Should().Be(55);
        }

        [StaFact]
        public void Should_RespectMaximum_WhenIncrementing()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 98, Increment = 5, Maximum = 100 };

            // Act
            nud.IncrementValue();

            // Assert
            nud.Value.Should().Be(100);
        }

        [StaFact]
        public void Should_NotIncrement_WhenReadOnly()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5, IsReadOnly = true };

            // Act
            nud.IncrementValue();

            // Assert
            nud.Value.Should().Be(50);
        }

        [StaFact]
        public void Should_ApplyDecimalPlaces_WhenIncrementing()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 10.123, Increment = 0.1, DecimalPlaces = 2 };

            // Act
            nud.IncrementValue();

            // Assert
            nud.Value.Should().BeApproximately(10.22, 0.01);
        }
    }

    public class DecrementValue_Method
    {
        [StaFact]
        public void Should_DecreaseValue_WhenDecrementValueCalled()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5, Minimum = 0 };

            // Act
            nud.DecrementValue();

            // Assert
            nud.Value.Should().Be(45);
        }

        [StaFact]
        public void Should_RespectMinimum_WhenDecrementing()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 2, Increment = 5, Minimum = 0 };

            // Act
            nud.DecrementValue();

            // Assert
            nud.Value.Should().Be(0);
        }

        [StaFact]
        public void Should_NotDecrement_WhenReadOnly()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5, IsReadOnly = true };

            // Act
            nud.DecrementValue();

            // Assert
            nud.Value.Should().Be(50);
        }

        [StaFact]
        public void Should_ApplyDecimalPlaces_WhenDecrementing()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 10.123, Increment = 0.1, DecimalPlaces = 2 };

            // Act
            nud.DecrementValue();

            // Assert
            nud.Value.Should().BeApproximately(10.02, 0.01);
        }
    }

    public class Commands
    {
        [StaFact]
        public void Should_ExecuteIncrementCommand()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5 };

            // Act
            nud.IncrementValueCommand.Execute(null);

            // Assert
            nud.Value.Should().Be(55);
        }

        [StaFact]
        public void Should_ExecuteDecrementCommand()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5 };

            // Act
            nud.DecrementValueCommand.Execute(null);

            // Assert
            nud.Value.Should().Be(45);
        }

        [StaFact]
        public void Should_NotExecuteIncrementCommand_WhenReadOnly()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5, IsReadOnly = true };

            // Act
            nud.IncrementValueCommand.Execute(null);

            // Assert
            nud.Value.Should().Be(50);
        }

        [StaFact]
        public void Should_NotExecuteDecrementCommand_WhenReadOnly()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5, IsReadOnly = true };

            // Act
            nud.DecrementValueCommand.Execute(null);

            // Assert
            nud.Value.Should().Be(50);
        }
    }

    public class ValueChanged_Event
    {
        [StaFact]
        public void Should_SubscribeToValueChangedEvent()
        {
            // Arrange
            var nud = new NumericUpDown();
            int eventCount = 0;
            nud.ValueChanged += (s, e) => eventCount++;

            // Act
            nud.Value = 25;

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var nud = new NumericUpDown();
            int eventCount1 = 0;
            int eventCount2 = 0;
            nud.ValueChanged += (s, e) => eventCount1++;
            nud.ValueChanged += (s, e) => eventCount2++;

            // Act
            nud.Value = 25;

            // Assert
            eventCount1.Should().Be(1);
            eventCount2.Should().Be(1);
        }

        [StaFact]
        public void Should_RaiseEventOnIncrement()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5 };
            int eventCount = 0;
            nud.ValueChanged += (s, e) => eventCount++;

            // Act
            nud.IncrementValue();

            // Assert
            eventCount.Should().Be(1);
        }

        [StaFact]
        public void Should_RaiseEventOnDecrement()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 5 };
            int eventCount = 0;
            nud.ValueChanged += (s, e) => eventCount++;

            // Act
            nud.DecrementValue();

            // Assert
            eventCount.Should().Be(1);
        }
    }

    public class EdgeCases
    {
        [StaFact]
        public void Should_HandleNegativeValues()
        {
            // Arrange
            var nud = new NumericUpDown { Minimum = -100, Maximum = 100 };

            // Act
            nud.Value = -50;

            // Assert
            nud.Value.Should().Be(-50);
        }

        [StaFact]
        public void Should_HandleLargeDecimalPlaces()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 12.3456789, DecimalPlaces = 4 };

            // Act
            nud.DecimalPlaces = 2;

            // Assert
            nud.Value.Should().BeApproximately(12.35, 0.01);
        }

        [StaFact]
        public void Should_HandleZeroIncrement()
        {
            // Arrange
            var nud = new NumericUpDown { Value = 50, Increment = 0 };

            // Act
            nud.IncrementValue();

            // Assert
            nud.Value.Should().Be(50);
        }

        [StaFact]
        public void Should_HandleMinimumEqualsMaximum()
        {
            // Arrange
            var nud = new NumericUpDown { Minimum = 50, Maximum = 50 };

            // Act
            nud.Value = 50;

            // Assert
            nud.Value.Should().Be(50);
            nud.Minimum.Should().Be(50);
            nud.Maximum.Should().Be(50);
        }
    }
}
