using FluentValidation;
using LibrarySystem.Application.Commands.Patrons;
using LibrarySystem.Application.Interfaces;

namespace LibrarySystem.Application.Validators.Patrons;

public class CreatePatronCommandValidator : AbstractValidator<CreatePatronCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePatronCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
            .MaximumLength(200).WithMessage("الاسم الكامل يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("الرقم الوطني مطلوب.")
            .MaximumLength(50).WithMessage("الرقم الوطني يجب ألا يتجاوز 50 حرفاً.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب.")
            .MaximumLength(50).WithMessage("رقم الهاتف يجب ألا يتجاوز 50 حرفاً.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("البريد الإلكتروني غير صالح.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.NationalId)
            .MustAsync(BeUniqueNationalId)
            .WithMessage("المستعير مسجل مسبقاً بهذا الرقم الوطني.");
    }

    private async Task<bool> BeUniqueNationalId(string nationalId, CancellationToken cancellationToken)
    {
        var patrons = await _unitOfWork.Patrons.GetAllAsync();
        return !patrons.Any(p => p.NationalId == nationalId);
    }
}
