using FluentValidation;
using LibrarySystem.Application.Commands.Media;

namespace LibrarySystem.Application.Validators.Media;

public class UpdateMediaCommandValidator : AbstractValidator<UpdateMediaCommand>
{
    public UpdateMediaCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("معرف الميديا غير صحيح");
        RuleFor(x => x.StoragePath).NotEmpty().WithMessage("المسار مطلوب للتعديل");
        RuleFor(x => x.FileName).NotEmpty().WithMessage("اسم الملف مطلوب للتعديل");
    }
}