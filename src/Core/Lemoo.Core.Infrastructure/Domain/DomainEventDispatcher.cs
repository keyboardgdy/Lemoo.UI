using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lemoo.Core.Infrastructure.Domain;

/// <summary>
/// Domain event dispatcher using MediatR
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerService _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILoggerService logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatch a single domain event
    /// </summary>
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

        await DispatchAsync(new[] { domainEvent }, cancellationToken);
    }

    /// <summary>
    /// Dispatch multiple domain events
    /// </summary>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null) throw new ArgumentNullException(nameof(domainEvents));

        var events = domainEvents.ToList();

        if (events.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Dispatching {Count} domain events", events.Count);

        foreach (var domainEvent in events)
        {
            try
            {
                _logger.LogDebug("Dispatching domain event: {EventType}", domainEvent.GetType().Name);

                // Use MediatR to publish the event to all handlers
                var mediator = _serviceProvider.GetRequiredService<IPublisher>();
                await mediator.Publish(domainEvent, cancellationToken);

                _logger.LogDebug("Successfully dispatched domain event: {EventType}", domainEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dispatching domain event: {EventType}", domainEvent.GetType().Name);
                throw;
            }
        }

        _logger.LogInformation("Successfully dispatched {Count} domain events", events.Count);
    }
}

/// <summary>
/// Generic domain event dispatcher for specific event types
/// </summary>
public class DomainEventDispatcher<T> : IDomainEventDispatcher<T> where T : IDomainEvent
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventDispatcher(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public async Task DispatchAsync(T domainEvent, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(domainEvent, cancellationToken);
    }
}
