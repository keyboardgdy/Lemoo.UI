using Lemoo.Core.Abstractions.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace Lemoo.Core.Infrastructure.Interceptors;

/// <summary>
/// EF Core interceptor for automatically populating audit fields
/// </summary>
public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AuditSaveChangesInterceptor(IHttpContextAccessor? httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        var currentUserId = GetCurrentUserId();
        var currentTime = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<Lemoo.Core.Domain.Entities.EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = currentTime;
                    entry.Entity.CreatedBy = currentUserId ?? string.Empty;
                    entry.Entity.UpdatedAt = currentTime;
                    entry.Entity.UpdatedBy = currentUserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = currentTime;
                    entry.Entity.UpdatedBy = currentUserId;

                    // Prevent modification of CreatedAt/CreatedBy
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;
            }
        }

        // Handle generic EntityBase<TKey> entities
        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            if (entry.Entity is IAuditable auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedAt = currentTime;
                        auditable.CreatedBy = currentUserId ?? string.Empty;
                        auditable.UpdatedAt = currentTime;
                        auditable.UpdatedBy = currentUserId;
                        break;

                    case EntityState.Modified:
                        auditable.UpdatedAt = currentTime;
                        auditable.UpdatedBy = currentUserId;
                        break;
                }
            }
        }
    }

    private string? GetCurrentUserId()
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        if (user == null) return null;

        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value;
    }
}

/// <summary>
/// Interface for entities that support audit tracking
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
