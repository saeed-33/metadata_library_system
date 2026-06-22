using FluentValidation;
using LibrarySystem.Application.Commands.ResourceTemplates;

namespace LibrarySystem.Application.Validators.ResourceTemplates;

public class CreateResourceTemplateCommandValidator : AbstractValidator<CreateResourceTemplateCommand>
{
    public CreateResourceTemplateCommandValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("اسم القالب مطلوب")
            .MaximumLength(100).WithMessage("الاسم طويل جداً");

        RuleFor(x => x.DefaultBorrowDays)
            .GreaterThan(0).WithMessage("عدد أيام الإعارة يجب أن يكون أكبر من صفر")
            .When(x => x.DefaultBorrowDays.HasValue);
    }
}