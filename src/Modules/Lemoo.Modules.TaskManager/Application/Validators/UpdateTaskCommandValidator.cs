using FluentValidation;
using Lemoo.Modules.TaskManager.Application.Commands;

namespace Lemoo.Modules.TaskManager.Application.Validators;

/// <summary>
/// 更新任务命令验证器
/// </summary>
public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("任务ID不能为空");
        
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("任务标题不能为空")
            .MaximumLength(200).WithMessage("任务标题不能超过200个字符");
        
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("任务描述不能超过1000个字符")
            .When(x => !string.IsNullOrEmpty(x.Description));
        
        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("截止日期必须大于当前时间")
            .When(x => x.DueDate.HasValue);
    }
}
