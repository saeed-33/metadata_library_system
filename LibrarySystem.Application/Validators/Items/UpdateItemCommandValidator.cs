using FluentValidation;
using LibrarySystem.Application.Commands.Items;

namespace LibrarySystem.Application.Validators.Items;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        // التأكد من معرف الكتاب
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرف الكتاب مطلوب")
            .GreaterThan(0).WithMessage("المعرف يجب أن يكون رقماً صحيحاً");

        // التأكد من وجود قيم وصفية (Metadata)
        RuleFor(x => x.Values)
            .NotEmpty().WithMessage("لا يمكن ترك الكتاب بدون بيانات وصفية")
            .Must(v => v.Count > 0).WithMessage("يجب إرسال قيمة واحدة على الأقل");

        // التحقق من صحة كل قيمة مرسلة في القائمة
        RuleForEach(x => x.Values).ChildRules(val => {
            val.RuleFor(v => v.PropertyId).GreaterThan(0).WithMessage("معرف الخاصية غير صحيح");
            val.RuleFor(v => v.ValueText).NotEmpty().WithMessage("نص القيمة لا يمكن أن يكون فارغاً");
            val.RuleFor(v => v.Type).Must(t => new[] { "literal", "uri", "resource" }.Contains(t))
                .WithMessage("نوع القيمة غير مدعوم");
        });
    }
}

/*استخدمنا 
 * RuleForEach
 * للتحقق من مصفوفة القيم المرسلة
 * (Nested Objects). 
 * هذا يضمن أن المستخدم لا يمكنه تعديل كتاب وإدخال روابط
 * (URIs) غير صحيحة أو ترك حقول إجبارية فارغة، مما يحافظ على جودة الـ
 * Metadata Standards
 * المتبعة في النظام."*/