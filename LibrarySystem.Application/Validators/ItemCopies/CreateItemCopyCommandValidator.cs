using FluentValidation;
using LibrarySystem.Application.Commands.ItemCopies;
using LibrarySystem.Application.Interfaces;

namespace LibrarySystem.Application.Validators.ItemCopies;

public class CreateItemCopyCommandValidator : AbstractValidator<CreateItemCopyCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateItemCopyCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("معرف الكتاب غير صالح.");

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("الباركود مطلوب.")
            .MaximumLength(100).WithMessage("الباركود يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.Barcode)
            .MustAsync(BeUniqueBarcode)
            .WithMessage("هذا الباركود مستخدم مسبقاً.");

        RuleFor(x => x.ItemId)
            .MustAsync(ItemExists)
            .WithMessage("الكتاب غير موجود.");
    }

    private async Task<bool> BeUniqueBarcode(string barcode, CancellationToken cancellationToken)
    {
        var copies = await _unitOfWork.ItemCopies.GetAllAsync();
        return !copies.Any(c => c.Barcode == barcode);
    }

    private async Task<bool> ItemExists(int itemId, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(itemId);
        return item != null;
    }
}
