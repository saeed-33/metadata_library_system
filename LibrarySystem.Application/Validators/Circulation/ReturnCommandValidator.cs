using FluentValidation;
using LibrarySystem.Application.Commands.Circulation;

namespace LibrarySystem.Application.Validators.Circulation;

public class ReturnCommandValidator : AbstractValidator<ReturnCommand>
{
    public ReturnCommandValidator()
    {
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("الباركود مطلوب.");
    }
}
