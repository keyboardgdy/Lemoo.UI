using FluentValidation;
using Lemoo.Core.Application.Common;

namespace Lemoo.Core.Application.Validators;

/// <summary>
/// PagedQuery验证器 - 自动验证和规范化分页参数
/// </summary>
/// <typeparam name="TQuery">查询类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class PagedQueryValidator<TQuery, TResponse> : AbstractValidator<TQuery>
    where TQuery : PagedQuery<TResponse>
{
    public PagedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("页码必须大于0");
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("每页大小必须大于0")
            .LessThanOrEqualTo(x => x.MaxPageSize)
            .WithMessage($"每页大小不能超过{{MaxPageSize}}");
        
        RuleFor(x => x.SortDirection)
            .Must(direction => string.IsNullOrWhiteSpace(direction) || 
                             direction.ToLowerInvariant() == "asc" || 
                             direction.ToLowerInvariant() == "desc")
            .WithMessage("排序方向必须是 'asc' 或 'desc'");
    }
    
    /// <summary>
    /// 自定义验证逻辑，在验证前自动规范化
    /// </summary>
    public override FluentValidation.Results.ValidationResult Validate(ValidationContext<TQuery> context)
    {
        // 先规范化
        context.InstanceToValidate.Normalize();
        
        // 再验证
        return base.Validate(context);
    }
}

