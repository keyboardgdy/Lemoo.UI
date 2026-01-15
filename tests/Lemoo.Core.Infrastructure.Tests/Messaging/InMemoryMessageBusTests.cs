using FluentAssertions;
using Lemoo.Core.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lemoo.Core.Infrastructure.Tests.Messaging;

public class InMemoryMessageBusTests
{
    private readonly Mock<ILogger<InMemoryMessageBus>> _loggerMock;
    private readonly InMemoryMessageBus _messageBus;

    public InMemoryMessageBusTests()
    {
        _loggerMock = new Mock<ILogger<InMemoryMessageBus>>();
        _messageBus = new InMemoryMessageBus(_loggerMock.Object);
    }

    [Fact]
    public async Task PublishAsync_WhenHasSubscribers_ShouldCallAllHandlers()
    {
        // Arrange
        var message = new TestMessage { Id = 1, Content = "Test" };
        var handler1Called = false;
        var handler2Called = false;

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            handler1Called = true;
            await Task.CompletedTask;
        });

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            handler2Called = true;
            await Task.CompletedTask;
        });

        // Act
        await _messageBus.PublishAsync(message);

        // Assert
        handler1Called.Should().BeTrue();
        handler2Called.Should().BeTrue();
    }

    [Fact]
    public async Task PublishAsync_WhenNoSubscribers_ShouldNotThrow()
    {
        // Arrange
        var message = new TestMessage { Id = 1, Content = "Test" };

        // Act & Assert
        var act = async () => await _messageBus.PublishAsync(message);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PublishAsync_WhenHandlerThrowsException_ShouldContinueProcessing()
    {
        // Arrange
        var message = new TestMessage { Id = 1, Content = "Test" };
        var handler2Called = false;

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            throw new InvalidOperationException("Test exception");
        });

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            handler2Called = true;
            await Task.CompletedTask;
        });

        // Act
        await _messageBus.PublishAsync(message);

        // Assert
        handler2Called.Should().BeTrue();
    }

    [Fact]
    public async Task PublishAsync_WhenMessageIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _messageBus.PublishAsync<TestMessage>(null!));
    }

    [Fact]
    public void Subscribe_ShouldReturnSubscriptionId()
    {
        // Arrange
        var subscriptionId = _messageBus.Subscribe<TestMessage>(async msg => await Task.CompletedTask);

        // Assert
        subscriptionId.Should().NotBeNullOrEmpty();
        subscriptionId.Should().StartWith("sub_");
    }

    [Fact]
    public async Task Subscribe_WhenSubscribed_ShouldReceivePublishedMessages()
    {
        // Arrange
        TestMessage? receivedMessage = null;

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            receivedMessage = msg;
            await Task.CompletedTask;
        });

        var message = new TestMessage { Id = 1, Content = "Test" };

        // Act
        await _messageBus.PublishAsync(message);

        // Assert
        receivedMessage.Should().NotBeNull();
        receivedMessage!.Id.Should().Be(1);
        receivedMessage.Content.Should().Be("Test");
    }

    [Fact]
    public void Subscribe_WhenHandlerIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _messageBus.Subscribe<TestMessage>(null!));
    }

    [Fact]
    public async Task Unsubscribe_ShouldRemoveHandler()
    {
        // Arrange
        var callCount = 0;
        var subscriptionId = _messageBus.Subscribe<TestMessage>(async msg =>
        {
            callCount++;
            await Task.CompletedTask;
        });

        var message = new TestMessage { Id = 1, Content = "Test" };

        // Act
        await _messageBus.PublishAsync(message);
        _messageBus.Unsubscribe(subscriptionId);
        await _messageBus.PublishAsync(message);

        // Assert
        callCount.Should().Be(1);
    }

    [Fact]
    public void Unsubscribe_WhenSubscriptionIdIsInvalid_ShouldNotThrow()
    {
        // Act & Assert
        var act = () => _messageBus.Unsubscribe("invalid-id");
        act.Should().NotThrow();
    }

    [Fact]
    public void Unsubscribe_WhenSubscriptionIdIsEmpty_ShouldNotThrow()
    {
        // Act & Assert
        var act = () => _messageBus.Unsubscribe(string.Empty);
        act.Should().NotThrow();

        var act2 = () => _messageBus.Unsubscribe(" ");
        act2.Should().NotThrow();
    }

    [Fact]
    public async Task UnsubscribeAll_ShouldRemoveAllHandlers()
    {
        // Arrange
        var handler1Called = false;
        var handler2Called = false;

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            handler1Called = true;
            await Task.CompletedTask;
        });

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            handler2Called = true;
            await Task.CompletedTask;
        });

        var message = new TestMessage { Id = 1, Content = "Test" };

        // Act
        _messageBus.UnsubscribeAll();
        await _messageBus.PublishAsync(message);

        // Assert
        handler1Called.Should().BeFalse();
        handler2Called.Should().BeFalse();
    }

    [Fact]
    public async Task Subscribe_WithDifferentMessageTypes_ShouldHandleSeparately()
    {
        // Arrange
        TestMessage? testMessageReceived = null;
        AnotherMessage? anotherMessageReceived = null;

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            testMessageReceived = msg;
            await Task.CompletedTask;
        });

        _messageBus.Subscribe<AnotherMessage>(async msg =>
        {
            anotherMessageReceived = msg;
            await Task.CompletedTask;
        });

        // Act
        await _messageBus.PublishAsync(new TestMessage { Id = 1, Content = "Test" });
        await _messageBus.PublishAsync(new AnotherMessage { Value = "Another" });

        // Assert
        testMessageReceived.Should().NotBeNull();
        anotherMessageReceived.Should().NotBeNull();
    }

    [Fact]
    public async Task Subscribe_MultipleHandlersSameType_ShouldCallAllInParallel()
    {
        // Arrange
        var executionOrder = new List<int>();
        var tcs1 = new TaskCompletionSource();
        var tcs2 = new TaskCompletionSource();

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            executionOrder.Add(1);
            await tcs1.Task;
            executionOrder.Add(2);
        });

        _messageBus.Subscribe<TestMessage>(async msg =>
        {
            executionOrder.Add(3);
            await tcs2.Task;
            executionOrder.Add(4);
        });

        // Act - Start publishing but don't complete tasks yet
        var publishTask = _messageBus.PublishAsync(new TestMessage { Id = 1, Content = "Test" });

        // Wait a bit to ensure handlers started
        await Task.Delay(50);

        // Both handlers should be executing (parallel)
        executionOrder.Should().Contain(1);
        executionOrder.Should().Contain(3);

        // Complete tasks
        tcs1.SetResult();
        tcs2.SetResult();
        await publishTask;

        // Assert - all steps should be completed
        executionOrder.Should().HaveCount(4);
    }

    [Fact]
    public async Task Unsubscribe_WhenMultipleSubscribers_ShouldOnlyRemoveUnsubscribed()
    {
        // Arrange
        var handler1Called = false;
        var handler2Called = false;

        var subscriptionId1 = _messageBus.Subscribe<TestMessage>(async msg =>
        {
            handler1Called = true;
            await Task.CompletedTask;
        });

        var subscriptionId2 = _messageBus.Subscribe<TestMessage>(async msg =>
        {
            handler2Called = true;
            await Task.CompletedTask;
        });

        // Act
        _messageBus.Unsubscribe(subscriptionId1);
        await _messageBus.PublishAsync(new TestMessage { Id = 1, Content = "Test" });

        // Assert
        handler1Called.Should().BeFalse();
        handler2Called.Should().BeTrue();
    }

    private class TestMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    private class AnotherMessage
    {
        public string Value { get; set; } = string.Empty;
    }
}
