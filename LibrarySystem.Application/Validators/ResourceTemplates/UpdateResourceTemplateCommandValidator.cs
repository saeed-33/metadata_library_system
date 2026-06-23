using FluentValidation;
using LibrarySystem.Application.Commands.ResourceTemplates;

namespace LibrarySystem.Application.Validators.ResourceTemplates;

public class UpdateResourceTemplateCommandValidator : AbstractValidator<UpdateResourceTemplateCommand>
{
    public UpdateResourceTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرف القالب مطلوب")
            .GreaterThan(0).WithMessage("المعرف يجب أن يكون رقماً صحيحاً");

        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("اسم القالب لا يمكن أن يكون فارغاً")
            .MaximumLength(100).WithMessage("اسم القالب لا يمكن أن يتجاوز 100 حرف");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("الوصف طويل جداً");

        RuleFor(x => x.DefaultBorrowDays)
            .GreaterThan(0).WithMessage("عدد أيام الإعارة يجب أن يكون أكبر من صفر")
            .When(x => x.DefaultBorrowDays.HasValue);
    }
}