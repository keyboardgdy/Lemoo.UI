namespace Lemoo.Core.Abstractions.Domain;

/// <summary>
/// Interface for entities that can hold domain events
/// </summary>
public interface IDomainEventContainer
{
    /// <summary>
    /// Gets all domain events
    /// </summary>
    IReadOnlyList<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Clears all domain events
    /// </summary>
    void ClearDomainEvents();

    /// <summary>
    /// Adds a domain event
    /// </summary>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Removes a domain event
    /// </summary>
    void RemoveDomainEvent(IDomainEvent domainEvent);
}
