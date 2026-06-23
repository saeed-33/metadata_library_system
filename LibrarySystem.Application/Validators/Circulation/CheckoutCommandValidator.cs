using FluentValidation;
using LibrarySystem.Application.Commands.Circulation;
using LibrarySystem.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.DTOs.Circulation;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        // ==========================================
        // 1. Basic Input Validation (From your file)
        // ==========================================
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("Barcode is required.");

        RuleFor(x => x.PatronId)
            .GreaterThan(0).WithMessage("Patron ID must be valid.");

        RuleFor(x => x.CustomDueDate)
            .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
            .WithMessage("Due date cannot be in the past.");

        // ==========================================
        // 2. Business Logic Validation (Database Checks)
        // ==========================================
        RuleFor(x => x.Barcode)
            .MustAsync(BeAnAvailableCopyAsync)
            .WithMessage("This copy does not exist or is currently not available (borrowed or under maintenance).");

        RuleFor(x => x.Barcode)
            .MustAsync(BeBorrowableTemplateAsync)
            .WithMessage("This item is for reference only and cannot be borrowed, or the book is missing a template.");
    }

    private async Task<bool> BeAnAvailableCopyAsync(string barcode, CancellationToken cancellationToken)
    {
        var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.Barcode == barcode);
        var copy = copies.FirstOrDefault();

        if (copy == null) return false;
        if (copy.Status != 0) return false; // 0 = Available

        return true;
    }

    private async Task<bool> BeBorrowableTemplateAsync(string barcode, CancellationToken cancellationToken)
    {
        var copies = await _unitOfWork.ItemCopies.FindAsync(c => c.Barcode == barcode);
        var copy = copies.FirstOrDefault();

        // If copy is null, let the previous rule handle the error. We return true here to avoid duplicate error messages.
        if (copy == null) return true;

        var item = await _unitOfWork.Items.GetByIdAsync(copy.ItemId);
        if (item == null || !item.TemplateId.HasValue) return false;

        var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item.TemplateId.Value);
        if (template == null || !template.IsBorrowable) return false;

        return true;
    }
}