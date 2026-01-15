using System.Linq.Expressions;

namespace Lemoo.Core.Abstractions.Specifications;

/// <summary>
/// Specification pattern interface for complex queries
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// The criteria expression
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Include expressions for navigation properties
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Include strings for complex navigation properties
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Order by expression
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Order by descending expression
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Group by expression
    /// </summary>
    Expression<Func<T, object>>? GroupBy { get; }

    /// <summary>
    /// Skip count for pagination
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Take count for pagination
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Whether the results are distinct
    /// </summary>
    bool IsDistinct { get; }

    /// <summary>
    /// Pagination enabled flag
    /// </summary>
    bool IsPagingEnabled { get; }
}

/// <summary>
/// Base specification implementation
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    protected Specification() { }

    protected Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public Expression<Func<T, bool>>? Criteria { get; private set; }

    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();

    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public Expression<Func<T, object>>? GroupBy { get; private set; }

    public int? Skip { get; private set; }
    public int? Take { get; private set; }
    public bool IsDistinct { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Add criteria
    /// </summary>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = Criteria is null
            ? criteria
            : Criteria.And(criteria);
    }

    /// <summary>
    /// Add include expression
    /// </summary>
    public void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Add include string for complex navigation
    /// </summary>
    public void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Apply ordering
    /// </summary>
    public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Apply ordering descending
    /// </summary>
    public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Apply grouping
    /// </summary>
    public void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
        GroupBy = groupByExpression;
    }

    /// <summary>
    /// Apply paging
    /// </summary>
    public void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Apply distinct
    /// </summary>
    public void ApplyDistinct()
    {
        IsDistinct = true;
    }
}

/// <summary>
/// Extension methods for combining expressions
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Combines two expressions with AND
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);

        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left, right), parameter);
    }

    /// <summary>
    /// Combines two expressions with OR
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);

        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(left, right), parameter);
    }

    /// <summary>
    /// Negates an expression
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var parameter = Expression.Parameter(typeof(T));

        var visitor = new ReplaceExpressionVisitor(expression.Parameters[0], parameter);
        var body = visitor.Visit(expression.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.Not(body!), parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}
