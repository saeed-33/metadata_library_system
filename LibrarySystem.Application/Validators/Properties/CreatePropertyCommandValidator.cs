using FluentValidation;
using LibrarySystem.Application.Commands.Properties;

namespace LibrarySystem.Application.Validators.Properties;

public class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
{
    public CreatePropertyCommandValidator()
    {
        RuleFor(x => x.VocabularyId).NotEmpty().WithMessage("الخاصية يجب أن تتبع قاموساً معيناً");
        RuleFor(x => x.LocalName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Label).NotEmpty().MaximumLength(100);

        // التحقق من أن الرابط هو URL صحيح
        RuleFor(x => x.TermUri)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("الرابط العالمي للمصطلح غير صحيح");
    }
}

/*Must: تتيح لك كتابة شرط خاص بك(Custom Logic).
Uri.TryCreate: وظيفتها محاولة بناء رابط(URL) من النص المدخل.
UriKind.Absolute: تشترط أن يكون الرابط كاملاً(أي يحتوي على البروتوكول مثل https:// وليس مجرد مسار فرعي).
out _: تعني أننا نريد فقط التأكد من نجاح العملية ولا نحتاج لتخزين النتيجة في متغير.*/