using FluentValidation;
using LibrarySystem.Application.Commands.ItemCopies;
using LibrarySystem.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        // Database Checks
        RuleFor(x => x.Barcode)
            .MustAsync(BeUniqueBarcode)
            .WithMessage("هذا الباركود مستخدم مسبقاً.");

        RuleFor(x => x.ItemId)
            .MustAsync(ItemExists)
            .WithMessage("الكتاب غير موجود.");

        RuleFor(x => x.ItemId)
            .MustAsync(ItemMustHaveTemplate)
            .WithMessage("هذا الكتاب غير مرتبط بقالب مصادر.");
    }

    private async Task<bool> BeUniqueBarcode(string barcode, CancellationToken cancellationToken)
    {
        // PERFORMANCE FIX: Use FindAsync instead of GetAllAsync
        var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.Barcode == barcode);
        return !copies.Any();
    }

    private async Task<bool> ItemExists(int itemId, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(itemId);
        return item != null;
    }

    private async Task<bool> ItemMustHaveTemplate(int itemId, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(itemId);
        // Returns false if the item doesn't exist OR if it has no template
        return item != null && item.TemplateId.HasValue;
    }
}