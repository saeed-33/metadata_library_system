using FluentValidation;
using LibrarySystem.Application.Commands.Properties;

namespace LibrarySystem.Application.Validators.Properties;

public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
{
    public UpdatePropertyCommandValidator()
    {
        // 1. التحقق من المعرف (Id) - ضروري جداً في التحديث
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرف الخاصية مطلوب")
            .GreaterThan(0).WithMessage("معرف الخاصية يجب أن يكون رقماً صحيحاً أكبر من صفر");

        // 2. التحقق من تبعية القاموس
        RuleFor(x => x.VocabularyId)
            .NotEmpty().WithMessage("يجب تحديد القاموس الذي تتبعه الخاصية")
            .GreaterThan(0).WithMessage("معرف القاموس غير صحيح");

        // 3. التحقق من النصوص والأطوال (نفس قواعد الإضافة لضمان التناسق)
        RuleFor(x => x.LocalName)
            .NotEmpty().WithMessage("الاسم البرمجي مطلوب")
            .MaximumLength(100).WithMessage("الاسم البرمجي لا يمكن أن يتجاوز 100 حرف");

        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("الاسم المعروض مطلوب")
            .MaximumLength(100).WithMessage("الاسم المعروض لا يمكن أن يتجاوز 100 حرف");

        // 4. التحقق من صحة الرابط العالمي
        RuleFor(x => x.TermUri)
            .NotEmpty().WithMessage("الرابط العالمي (Term URI) مطلوب")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("يجب إدخال رابط عالمي (URI) صحيح");
    }
}