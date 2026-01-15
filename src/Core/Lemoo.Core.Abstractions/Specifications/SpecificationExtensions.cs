using Lemoo.Core.Abstractions.Persistence;

namespace Lemoo.Core.Abstractions.Specifications;

/// <summary>
/// 规范扩展方法
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// 应用分页到规范
    /// </summary>
    public static ISpecification<T> WithPaging<T>(this ISpecification<T> specification, PagedRequest request)
        where T : class
    {
        if (specification is Specification<T> spec)
        {
            spec.ApplyPaging(request.Skip, request.Take);
        }
        return specification;
    }

    /// <summary>
    /// 添加排序到规范
    /// </summary>
    public static ISpecification<T> WithOrderBy<T>(this ISpecification<T> specification, System.Linq.Expressions.Expression<Func<T, object>> orderBy)
        where T : class
    {
        if (specification is Specification<T> spec)
        {
            spec.ApplyOrderBy(orderBy);
        }
        return specification;
    }

    /// <summary>
    /// 添加降序排序到规范
    /// </summary>
    public static ISpecification<T> WithOrderByDescending<T>(this ISpecification<T> specification, System.Linq.Expressions.Expression<Func<T, object>> orderByDescending)
        where T : class
    {
        if (specification is Specification<T> spec)
        {
            spec.ApplyOrderByDescending(orderByDescending);
        }
        return specification;
    }

    /// <summary>
    /// 添加导航属性包含
    /// </summary>
    public static ISpecification<T> WithInclude<T>(this ISpecification<T> specification, System.Linq.Expressions.Expression<Func<T, object>> include)
        where T : class
    {
        if (specification is Specification<T> spec)
        {
            spec.AddInclude(include);
        }
        return specification;
    }

    /// <summary>
    /// 添加字符串导航属性包含
    /// </summary>
    public static ISpecification<T> WithInclude<T>(this ISpecification<T> specification, string include)
        where T : class
    {
        if (specification is Specification<T> spec)
        {
            spec.AddInclude(include);
        }
        return specification;
    }

    /// <summary>
    /// 应用去重
    /// </summary>
    public static ISpecification<T> AsDistinct<T>(this ISpecification<T> specification)
        where T : class
    {
        if (specification is Specification<T> spec)
        {
            spec.ApplyDistinct();
        }
        return specification;
    }
}

/// <summary>
/// 分页规范
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class PagedSpecification<T> : Specification<T>
    where T : class
{
    public PagedSpecification(PagedRequest request)
    {
        ApplyPaging(request.Skip, request.Take);
    }

    public PagedSpecification(System.Linq.Expressions.Expression<Func<T, bool>> criteria, PagedRequest request)
        : base(criteria)
    {
        ApplyPaging(request.Skip, request.Take);
    }

    public PagedSpecification<T> WithOrderBy(System.Linq.Expressions.Expression<Func<T, object>> orderBy)
    {
        ApplyOrderBy(orderBy);
        return this;
    }

    public PagedSpecification<T> WithOrderByDescending(System.Linq.Expressions.Expression<Func<T, object>> orderByDescending)
    {
        ApplyOrderByDescending(orderByDescending);
        return this;
    }

    public PagedSpecification<T> WithInclude(System.Linq.Expressions.Expression<Func<T, object>> include)
    {
        AddInclude(include);
        return this;
    }

    public PagedSpecification<T> WithInclude(string include)
    {
        AddInclude(include);
        return this;
    }
}

/// <summary>
/// 排序信息
/// </summary>
public record SortInfo(string PropertyName, bool IsDescending = false);

/// <summary>
/// 多排序规范
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class MultiOrderSpecification<T> : Specification<T>
    where T : class
{
    private readonly List<SortInfo> _sortInfo = new();

    public MultiOrderSpecification(params SortInfo[] sortInfo)
    {
        _sortInfo.AddRange(sortInfo);
    }

    public IReadOnlyList<SortInfo> SortInfo => _sortInfo.AsReadOnly();

    public MultiOrderSpecification<T> ThenBy(string propertyName, bool isDescending = false)
    {
        _sortInfo.Add(new SortInfo(propertyName, isDescending));
        return this;
    }
}

/// <summary>
/// 范围规范
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
/// <typeparam name="TProperty">属性类型</typeparam>
public class RangeSpecification<T, TProperty> : Specification<T>
    where T : class
    where TProperty : IComparable<TProperty>
{
    public RangeSpecification(
        System.Linq.Expressions.Expression<Func<T, TProperty>> property,
        TProperty? min,
        TProperty? max,
        bool minInclusive = true,
        bool maxInclusive = true)
    {
        var criteria = BuildRangeCriteria(property, min, max, minInclusive, maxInclusive);
        if (criteria != null)
        {
            AddCriteria(criteria);
        }
    }

    private static System.Linq.Expressions.Expression<Func<T, bool>>? BuildRangeCriteria(
        System.Linq.Expressions.Expression<Func<T, TProperty>> property,
        TProperty? min,
        TProperty? max,
        bool minInclusive,
        bool maxInclusive)
    {
        if (min == null && max == null)
            return null;

        var param = property.Parameters[0];

        System.Linq.Expressions.Expression? body = null;

        if (min != null)
        {
            var minValue = System.Linq.Expressions.Expression.Constant(min, typeof(TProperty));
            var minComparison = minInclusive
                ? System.Linq.Expressions.Expression.GreaterThanOrEqual(property.Body, minValue)
                : System.Linq.Expressions.Expression.GreaterThan(property.Body, minValue);
            body = minComparison;
        }

        if (max != null)
        {
            var maxValue = System.Linq.Expressions.Expression.Constant(max, typeof(TProperty));
            var maxComparison = maxInclusive
                ? System.Linq.Expressions.Expression.LessThanOrEqual(property.Body, maxValue)
                : System.Linq.Expressions.Expression.LessThan(property.Body, maxValue);

            body = body == null
                ? maxComparison
                : System.Linq.Expressions.Expression.AndAlso(body, maxComparison);
        }

        return body == null ? null : System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, param);
    }
}

/// <summary>
/// 搜索规范（模糊匹配）
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class SearchSpecification<T> : Specification<T>
    where T : class
{
    public SearchSpecification(
        System.Linq.Expressions.Expression<Func<T, string>> property,
        string searchTerm,
        bool caseInsensitive = true)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return;

        var searchExpression = BuildSearchExpression(property, searchTerm, caseInsensitive);
        if (searchExpression != null)
        {
            AddCriteria(searchExpression);
        }
    }

    private static System.Linq.Expressions.Expression<Func<T, bool>>? BuildSearchExpression(
        System.Linq.Expressions.Expression<Func<T, string>> property,
        string searchTerm,
        bool caseInsensitive)
    {
        var searchTermConstant = System.Linq.Expressions.Expression.Constant(
            caseInsensitive ? searchTerm.ToLower() : searchTerm);

        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        var propertyAccess = property.Body;

        if (caseInsensitive && toLowerMethod != null)
        {
            propertyAccess = System.Linq.Expressions.Expression.Call(propertyAccess, toLowerMethod);
        }

        var containsCall = System.Linq.Expressions.Expression.Call(propertyAccess, containsMethod!, searchTermConstant);

        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(containsCall, property.Parameters[0]);
    }
}

/// <summary>
/// 组合规范（AND 逻辑）
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class AndSpecification<T> : Specification<T>
    where T : class
{
    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        if (left.Criteria != null && right.Criteria != null)
        {
            AddCriteria(left.Criteria.And(right.Criteria));
        }
        else if (left.Criteria != null)
        {
            AddCriteria(left.Criteria);
        }
        else if (right.Criteria != null)
        {
            AddCriteria(right.Criteria);
        }

        // Merge includes
        if (left is Specification<T> leftSpec)
        {
            foreach (var include in leftSpec.Includes)
                AddInclude(include);
            foreach (var includeString in leftSpec.IncludeStrings)
                AddInclude(includeString);
        }

        if (right is Specification<T> rightSpec)
        {
            foreach (var include in rightSpec.Includes)
                AddInclude(include);
            foreach (var includeString in rightSpec.IncludeStrings)
                AddInclude(includeString);
        }
    }
}

/// <summary>
/// 组合规范（OR 逻辑）
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class OrSpecification<T> : Specification<T>
    where T : class
{
    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        if (left.Criteria != null && right.Criteria != null)
        {
            AddCriteria(left.Criteria.Or(right.Criteria));
        }
        else if (left.Criteria != null)
        {
            AddCriteria(left.Criteria);
        }
        else if (right.Criteria != null)
        {
            AddCriteria(right.Criteria);
        }
    }
}

/// <summary>
/// 否定规范
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public class NotSpecification<T> : Specification<T>
    where T : class
{
    public NotSpecification(ISpecification<T> specification)
    {
        if (specification.Criteria != null)
        {
            AddCriteria(specification.Criteria.Not());
        }

        if (specification is Specification<T> spec)
        {
            foreach (var include in spec.Includes)
                AddInclude(include);
            foreach (var includeString in spec.IncludeStrings)
                AddInclude(includeString);
        }
    }
}
