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
    }
}