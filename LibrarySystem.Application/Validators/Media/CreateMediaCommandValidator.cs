using FluentValidation;
using LibrarySystem.Application.Commands.Media;

namespace LibrarySystem.Application.Validators.Media;

public class CreateMediaCommandValidator : AbstractValidator<CreateMediaCommand>
{
    public CreateMediaCommandValidator()
    {
        RuleFor(x => x.ItemId).GreaterThan(0);
        RuleFor(x => x.StoragePath).NotEmpty().WithMessage("مسار التخزين مطلوب");
        RuleFor(x => x.FileName).NotEmpty().WithMessage("اسم الملف مطلوب");
    }
}