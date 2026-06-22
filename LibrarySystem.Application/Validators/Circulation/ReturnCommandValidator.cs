using FluentValidation;
using LibrarySystem.Application.Commands.Circulation;
using LibrarySystem.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Validators.Circulation;

public class ReturnCommandValidator : AbstractValidator<ReturnCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReturnCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("الباركود مطلوب.");

        RuleFor(x => x.Barcode)
            .MustAsync(CopyMustExistAsync)
            .WithMessage("النسخة غير موجودة.");

        RuleFor(x => x.Barcode)
            .MustAsync(HaveAnActiveBorrowRecordAsync)
            .WithMessage("لا يوجد سجل إعارة نشط لهذه النسخة.");
    }

    private async Task<bool> CopyMustExistAsync(string barcode, CancellationToken cancellationToken)
    {
        var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.Barcode == barcode);
        return copies.Any();
    }

    private async Task<bool> HaveAnActiveBorrowRecordAsync(string barcode, CancellationToken cancellationToken)
    {
        var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.Barcode == barcode);
        var copy = copies.FirstOrDefault();

        // If copy doesn't exist, return true here so we don't show duplicate errors. 
        // The 'CopyMustExistAsync' rule above will catch it.
        if (copy == null) return true;

        // Check if there is a record for this copy where ReturnDate is null
        var records = await _unitOfWork.BorrowRecords.FindAsync(r => r.CopyId == copy.Id && r.ReturnDate == null);

        return records.Any();
    }
}