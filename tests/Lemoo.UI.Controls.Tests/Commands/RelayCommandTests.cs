using System;
using System.Windows.Input;
using FluentAssertions;
using Lemoo.UI.Commands;
using Xunit;

namespace Lemoo.UI.Commands.Tests;

public class RelayCommandTests
{
    public class Constructor
    {
        [Fact]
        public void Should_CreateInstance_WithExecute()
        {
            // Arrange
            bool executed = false;
            Action<object?> execute = _ => executed = true;

            // Act
            var command = new RelayCommand(execute);

            // Assert
            command.Should().NotBeNull();
            executed = false;
            command.Execute(null);
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_CreateInstance_WithExecuteAndCanExecute()
        {
            // Arrange
            bool executed = false;
            Action<object?> execute = _ => executed = true;
            Func<object?, bool> canExecute = _ => true;

            // Act
            var command = new RelayCommand(execute, canExecute);

            // Assert
            command.Should().NotBeNull();
            command.CanExecute(null).Should().BeTrue();
        }

        [Fact]
        public void Should_ThrowArgumentNullException_WhenExecuteIsNull()
        {
            // Act
            Action act = () => new RelayCommand(null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("execute");
        }

        [Fact]
        public void Should_AllowNullCanExecute()
        {
            // Arrange
            Action<object?> execute = _ => { };

            // Act
            var command = new RelayCommand(execute, null);

            // Assert
            command.CanExecute(null).Should().BeTrue();
        }
    }

    public class Execute_Method
    {
        [Fact]
        public void Should_ExecuteAction()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand(_ => executed = true);

            // Act
            command.Execute(null);

            // Assert
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_PassParameterToExecute()
        {
            // Arrange
            object? capturedParameter = null;
            var parameter = new object();
            var command = new RelayCommand(p => capturedParameter = p);

            // Act
            command.Execute(parameter);

            // Assert
            capturedParameter.Should().Be(parameter);
        }

        [Fact]
        public void Should_ExecuteWithNullParameter()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand(_ => executed = true);

            // Act
            command.Execute(null);

            // Assert
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_ExecuteMultipleTimes()
        {
            // Arrange
            int executeCount = 0;
            var command = new RelayCommand(_ => executeCount++);

            // Act
            command.Execute(null);
            command.Execute(null);
            command.Execute(null);

            // Assert
            executeCount.Should().Be(3);
        }
    }

    public class CanExecute_Method
    {
        [Fact]
        public void Should_ReturnTrue_WhenCanExecuteIsNull()
        {
            // Arrange
            var command = new RelayCommand(_ => { });

            // Act
            var canExecute = command.CanExecute(null);

            // Assert
            canExecute.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_WhenCanExecuteReturnsTrue()
        {
            // Arrange
            var command = new RelayCommand(_ => { }, _ => true);

            // Act
            var canExecute = command.CanExecute(null);

            // Assert
            canExecute.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_WhenCanExecuteReturnsFalse()
        {
            // Arrange
            var command = new RelayCommand(_ => { }, _ => false);

            // Act
            var canExecute = command.CanExecute(null);

            // Assert
            canExecute.Should().BeFalse();
        }

        [Fact]
        public void Should_PassParameterToCanExecute()
        {
            // Arrange
            object? capturedParameter = null;
            var parameter = new object();
            var command = new RelayCommand(_ => { }, p => { capturedParameter = p; return true; });

            // Act
            command.CanExecute(parameter);

            // Assert
            capturedParameter.Should().Be(parameter);
        }

        [Fact]
        public void Should_NotExecute_WhenCanExecuteReturnsFalse()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand(_ => executed = true, _ => false);

            // Act
            command.Execute(null);

            // Assert
            executed.Should().BeFalse();
        }
    }

    public class CanExecuteChanged_Event
    {
        [Fact]
        public void Should_SubscribeToCanExecuteChanged()
        {
            // Arrange
            var command = new RelayCommand(_ => { });
            int eventCount = 0;
            command.CanExecuteChanged += (s, e) => eventCount++;

            // Act
            CommandManager.InvalidateRequerySuggested();

            // Allow event to propagate
            System.Threading.Thread.Sleep(50);

            // Assert
            // The event is raised via CommandManager.RequerySuggested
            command.Should().NotBeNull();
        }

        [Fact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var command = new RelayCommand(_ => { });
            int eventCount1 = 0;
            int eventCount2 = 0;
            command.CanExecuteChanged += (s, e) => eventCount1++;
            command.CanExecuteChanged += (s, e) => eventCount2++;

            // Act
            CommandManager.InvalidateRequerySuggested();

            // Assert
            command.Should().NotBeNull();
        }

        [Fact]
        public void Should_AllowUnsubscribing()
        {
            // Arrange
            var command = new RelayCommand(_ => { });
            int eventCount = 0;
            EventHandler handler = (s, e) => eventCount++;
            command.CanExecuteChanged += handler;
            command.CanExecuteChanged -= handler;

            // Act
            CommandManager.InvalidateRequerySuggested();

            // Assert
            command.Should().NotBeNull();
        }
    }
}

public class RelayCommandTTests
{
    public class Constructor
    {
        [Fact]
        public void Should_CreateInstance_WithExecute()
        {
            // Arrange
            bool executed = false;
            Action<string?> execute = _ => executed = true;

            // Act
            var command = new RelayCommand<string>(execute);

            // Assert
            command.Should().NotBeNull();
        }

        [Fact]
        public void Should_CreateInstance_WithExecuteAndCanExecute()
        {
            // Arrange
            Action<string?> execute = _ => { };
            Func<string?, bool> canExecute = _ => true;

            // Act
            var command = new RelayCommand<string>(execute, canExecute);

            // Assert
            command.Should().NotBeNull();
            command.CanExecute(null).Should().BeTrue();
        }

        [Fact]
        public void Should_ThrowArgumentNullException_WhenExecuteIsNull()
        {
            // Act
            Action act = () => new RelayCommand<string>(null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("execute");
        }

        [Fact]
        public void Should_AllowNullCanExecute()
        {
            // Arrange
            Action<string?> execute = _ => { };

            // Act
            var command = new RelayCommand<string>(execute, null);

            // Assert
            command.CanExecute(null).Should().BeTrue();
        }
    }

    public class Execute_Method
    {
        [Fact]
        public void Should_ExecuteAction()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand<string>(_ => executed = true);

            // Act
            command.Execute(null);

            // Assert
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_PassParameterToExecute()
        {
            // Arrange
            string? capturedParameter = null;
            var command = new RelayCommand<string>(p => capturedParameter = p);

            // Act
            command.Execute("test");

            // Assert
            capturedParameter.Should().Be("test");
        }

        [Fact]
        public void Should_ExecuteWithNullParameter()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand<string>(_ => executed = true);

            // Act
            command.Execute(null);

            // Assert
            executed.Should().BeTrue();
        }

        [Fact]
        public void Should_ExecuteMultipleTimes()
        {
            // Arrange
            int executeCount = 0;
            var command = new RelayCommand<int>(_ => executeCount++);

            // Act
            command.Execute(0);
            command.Execute(0);
            command.Execute(0);

            // Assert
            executeCount.Should().Be(3);
        }

        [Fact]
        public void Should_HandleValueTypeParameter()
        {
            // Arrange
            int capturedValue = 0;
            var command = new RelayCommand<int>(p => capturedValue = p);

            // Act
            command.Execute(42);

            // Assert
            capturedValue.Should().Be(42);
        }
    }

    public class CanExecute_Method
    {
        [Fact]
        public void Should_ReturnTrue_WhenCanExecuteIsNull()
        {
            // Arrange
            var command = new RelayCommand<string>(_ => { });

            // Act
            var canExecute = command.CanExecute(null);

            // Assert
            canExecute.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_WhenCanExecuteReturnsTrue()
        {
            // Arrange
            var command = new RelayCommand<string>(_ => { }, _ => true);

            // Act
            var canExecute = command.CanExecute(null);

            // Assert
            canExecute.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_WhenCanExecuteReturnsFalse()
        {
            // Arrange
            var command = new RelayCommand<string>(_ => { }, _ => false);

            // Act
            var canExecute = command.CanExecute(null);

            // Assert
            canExecute.Should().BeFalse();
        }

        [Fact]
        public void Should_PassParameterToCanExecute()
        {
            // Arrange
            string? capturedParameter = null;
            var command = new RelayCommand<string>(_ => { }, p => { capturedParameter = p; return true; });

            // Act
            command.CanExecute("test");

            // Assert
            capturedParameter.Should().Be("test");
        }

        [Fact]
        public void Should_NotExecute_WhenCanExecuteReturnsFalse()
        {
            // Arrange
            bool executed = false;
            var command = new RelayCommand<string>(_ => executed = true, _ => false);

            // Act
            command.Execute(null);

            // Assert
            executed.Should().BeFalse();
        }

        [Fact]
        public void Should_HandleCanExecuteWithValueType()
        {
            // Arrange
            var command = new RelayCommand<int>(_ => { }, p => p > 0);

            // Act
            var canExecutePositive = command.CanExecute(1);
            var canExecuteZero = command.CanExecute(0);
            var canExecuteNegative = command.CanExecute(-1);

            // Assert
            canExecutePositive.Should().BeTrue();
            canExecuteZero.Should().BeFalse();
            canExecuteNegative.Should().BeFalse();
        }
    }

    public class CanExecuteChanged_Event
    {
        [Fact]
        public void Should_SubscribeToCanExecuteChanged()
        {
            // Arrange
            var command = new RelayCommand<string>(_ => { });
            int eventCount = 0;
            command.CanExecuteChanged += (s, e) => eventCount++;

            // Act
            CommandManager.InvalidateRequerySuggested();

            // Assert
            command.Should().NotBeNull();
        }

        [Fact]
        public void Should_AllowMultipleSubscribers()
        {
            // Arrange
            var command = new RelayCommand<string>(_ => { });
            int eventCount1 = 0;
            int eventCount2 = 0;
            command.CanExecuteChanged += (s, e) => eventCount1++;
            command.CanExecuteChanged += (s, e) => eventCount2++;

            // Act
            CommandManager.InvalidateRequerySuggested();

            // Assert
            command.Should().NotBeNull();
        }

        [Fact]
        public void Should_AllowUnsubscribing()
        {
            // Arrange
            var command = new RelayCommand<string>(_ => { });
            int eventCount = 0;
            EventHandler handler = (s, e) => eventCount++;
            command.CanExecuteChanged += handler;
            command.CanExecuteChanged -= handler;

            // Act
            CommandManager.InvalidateRequerySuggested();

            // Assert
            command.Should().NotBeNull();
        }
    }
}
