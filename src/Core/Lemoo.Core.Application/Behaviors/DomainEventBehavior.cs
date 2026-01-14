using Lemoo.Core.Abstractions.CQRS;
using Lemoo.Core.Abstractions.Domain;
using Lemoo.Core.Abstractions.Logging;
using Lemoo.Core.Abstractions.Persistence;
using MediatR;

namespace Lemoo.Core.Application.Behaviors;

/// <summary>
/// Pipeline behavior that automatically dispatches domain events after command execution
/// </summary>
public class DomainEventBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly ILoggerService _logger;

    public DomainEventBehavior(
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher,
        ILoggerService logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _domainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only process commands, not queries
        // Check if request implements IQuery<> interface (any generic version)
        var requestType = request.GetType();
        var isQuery = requestType.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

        if (isQuery)
        {
            return await next();
        }

        // Execute the request handler
        var response = await next();

        // Dispatch domain events after successful execution
        await DispatchDomainEventsAsync(cancellationToken);

        return response;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregatesWithEvents = new List<IDomainEventContainer>();

        // Get all entities with domain events from tracked entities
        var entries = _unitOfWork.GetTrackedEntities();
        foreach (var entry in entries)
        {
            if (entry is IDomainEventContainer container)
            {
                var events = container.GetDomainEvents();
                if (events.Count > 0)
                {
                    aggregatesWithEvents.Add(container);
                }
            }
        }

        var allEvents = aggregatesWithEvents
            .SelectMany(x => x.GetDomainEvents())
            .OrderBy(x => x.OccurredOn)
            .ToList();

        if (allEvents.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Dispatching {Count} domain events for {RequestType}", allEvents.Count, typeof(TRequest).Name);

        // Dispatch all events
        await _domainEventDispatcher.DispatchAsync(allEvents, cancellationToken);

        // Clear events from aggregates after successful dispatch
        foreach (var aggregate in aggregatesWithEvents)
        {
            aggregate.ClearDomainEvents();
        }

        _logger.LogInformation("Successfully dispatched and cleared {Count} domain events", allEvents.Count);
    }
}
