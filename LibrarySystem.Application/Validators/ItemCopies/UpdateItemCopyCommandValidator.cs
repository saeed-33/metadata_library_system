using FluentValidation;
using LibrarySystem.Application.Commands.ItemCopies;
using LibrarySystem.Domain.Enums;

namespace LibrarySystem.Application.Validators.ItemCopies;

public class UpdateItemCopyCommandValidator : AbstractValidator<UpdateItemCopyCommand>
{
    public UpdateItemCopyCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("معرف النسخة غير صالح.");

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("الباركود مطلوب.")
            .MaximumLength(100).WithMessage("الباركود يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("حالة النسخة غير صالحة.");
    }
}
