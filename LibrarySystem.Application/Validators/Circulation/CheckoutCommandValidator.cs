using FluentValidation;

namespace LibrarySystem.Application.Validators.Circulation;
public class CheckoutCommandValidator : AbstractValidator<Commands.Features.CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(x => x.Barcode).NotEmpty().WithMessage("Barcode is required.");
        RuleFor(x => x.PatronId).GreaterThan(0);
        RuleFor(x => x.CustomDueDate)
            .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
            .WithMessage("Due date cannot be in the past.");
    }
}