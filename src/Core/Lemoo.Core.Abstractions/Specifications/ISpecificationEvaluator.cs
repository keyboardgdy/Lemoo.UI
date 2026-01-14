using System.Linq.Expressions;

namespace Lemoo.Core.Abstractions.Specifications;

/// <summary>
/// Specification evaluator interface for applying specifications to queries
/// </summary>
public interface ISpecificationEvaluator
{
    /// <summary>
    /// Get query with specification applied
    /// </summary>
    IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> input, ISpecification<T> specification)
        where T : class
        where TResult : class;

    /// <summary>
    /// Get query with specification applied
    /// </summary>
    IQueryable<T> GetQuery<T>(IQueryable<T> input, ISpecification<T> specification)
        where T : class;
}

/// <summary>
/// Default specification evaluator implementation
/// </summary>
public class SpecificationEvaluator : ISpecificationEvaluator
{
    public static SpecificationEvaluator Instance { get; } = new SpecificationEvaluator();

    public IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> input, ISpecification<T> specification)
        where T : class
        where TResult : class
    {
        var query = GetQuery(input, specification);

        // Handle select transformation if needed
        // This is a placeholder - actual implementation depends on your needs
        return (IQueryable<TResult>)query;
    }

    public IQueryable<T> GetQuery<T>(IQueryable<T> input, ISpecification<T> specification)
        where T : class
    {
        var query = input;

        // Apply criteria filter
        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes
        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }

        // Apply string includes for complex navigation
        foreach (var includeString in specification.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        // Apply ordering
        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply grouping
        if (specification.GroupBy is not null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(g => g);
        }

        // Apply paging
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip ?? 0).Take(specification.Take ?? 10);
        }

        // Apply distinct
        if (specification.IsDistinct)
        {
            query = query.Distinct();
        }

        return query;
    }
}

/// <summary>
/// Helper for including navigation properties
/// </summary>
public static class IncludeHelper
{
    /// <summary>
    /// Include a navigation property
    /// </summary>
    public static IQueryable<T> Include<T>(this IQueryable<T> source, Expression<Func<T, object>> includeExpression)
        where T : class
    {
        // This would use EF Core's Include in actual implementation
        // For now, return the source as-is
        return source;
    }

    /// <summary>
    /// Include a navigation property by string path
    /// </summary>
    public static IQueryable<T> Include<T>(this IQueryable<T> source, string includeString)
        where T : class
    {
        // This would use EF Core's Include in actual implementation
        // For now, return the source as-is
        return source;
    }
}
