using FluentValidation;
using LibrarySystem.Application.Commands.Items;

namespace LibrarySystem.Application.Validators.Items;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        // التحقق من وجود مالك وقالب (اختياري حسب منطق عملك)
        RuleFor(x => x.OwnerId).NotEmpty().WithMessage("يجب تحديد صاحب السجل");

        // التحقق من قائمة القيم
        RuleFor(x => x.Values)
            .NotEmpty().WithMessage("لا يمكن إنشاء عنصر بدون بيانات وصفية (Values) على الأقل قيمة واحدة");

        // التحقق من كل قيمة داخل القائمة
        RuleForEach(x => x.Values).ChildRules(val => {
            val.RuleFor(v => v.PropertyId).GreaterThan(0);
            val.RuleFor(v => v.ValueText).NotEmpty().WithMessage("نص القيمة مطلوب");
        });
    }
}