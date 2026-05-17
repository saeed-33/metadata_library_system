using FluentValidation;
using LibrarySystem.Application.Commands.ResourceTemplates;

namespace LibrarySystem.Application.Validators.ResourceTemplates;

public class UpdateResourceTemplateCommandValidator : AbstractValidator<UpdateResourceTemplateCommand>
{
    public UpdateResourceTemplateCommandValidator()
    {
        // ضروري جداً للتأكد من أننا نحدث سجلاً موجوداً فعلاً
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرف القالب مطلوب")
            .GreaterThan(0).WithMessage("المعرف يجب أن يكون رقماً صحيحاً");

        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("اسم القالب لا يمكن أن يكون فارغاً")
            .MaximumLength(100).WithMessage("اسم القالب لا يمكن أن يتجاوز 100 حرف");

        // الوصف اختياري ولكن إذا وُجد يجب ألا يتجاوز طولاً معيناً
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("الوصف طويل جداً");
    }
}