using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lemoo.Core.Infrastructure.Interceptors;

/// <summary>
/// EF Core interceptor for implementing soft delete pattern
/// </summary>
public class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        HandleSoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void HandleSoftDelete(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Deleted, Entity: ISoftDeletable softDeletable })
            {
                // Instead of deleting, mark as deleted
                entry.State = EntityState.Modified;
                softDeletable.IsDeleted = true;
                softDeletable.DeletedAt = DateTime.UtcNow;

                // Prevent cascade delete
                foreach (var navigation in entry.Navigations)
                {
                    if (!navigation.IsCollection && navigation.Metadata.IsOnDependent)
                    {
                        navigation.TargetEntry!.State = EntityState.Modified;
                    }
                }
            }
        }
    }
}

/// <summary>
/// Query filter interceptor for automatically filtering soft-deleted entities
/// </summary>
public static class SoftDeleteQueryFilterInterceptor
{
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                // Configure query filter to exclude soft-deleted entities
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(typeof(SoftDeleteQueryFilterInterceptor)
                        .GetMethod(nameof(GetSoftDeleteFilter))!
                        .MakeGenericMethod(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.Expression<Func<T, bool>> GetSoftDeleteFilter<T>()
        where T : class, ISoftDeletable
    {
        return x => !x.IsDeleted;
    }
}

/// <summary>
/// Interface for entities that support soft delete
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Indicates whether the entity has been soft deleted
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Timestamp when the entity was soft deleted
    /// </summary>
    DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Extension methods for working with soft deletable entities
/// </summary>
public static class SoftDeletableExtensions
{
    /// <summary>
    /// Include soft-deleted entities in the query
    /// </summary>
    public static IQueryable<T> IncludeDeleted<T>(this DbSet<T> dbSet)
        where T : class, ISoftDeletable
    {
        return dbSet.IgnoreQueryFilters();
    }

    /// <summary>
    /// Get only soft-deleted entities
    /// </summary>
    public static IQueryable<T> OnlyDeleted<T>(this DbSet<T> dbSet)
        where T : class, ISoftDeletable
    {
        return dbSet.IgnoreQueryFilters().Where(x => x.IsDeleted);
    }

    /// <summary>
    /// Permanently delete an entity (hard delete)
    /// </summary>
    public static void HardDelete<T>(this DbSet<T> dbSet, T entity)
        where T : class, ISoftDeletable
    {
        var entry = dbSet.Attach(entity);
        entry.State = EntityState.Deleted;
    }

    /// <summary>
    /// Restore a soft-deleted entity
    /// </summary>
    public static void Restore<T>(this DbSet<T> dbSet, T entity)
        where T : class, ISoftDeletable
    {
        entity.IsDeleted = false;
        entity.DeletedAt = null;
    }
}
