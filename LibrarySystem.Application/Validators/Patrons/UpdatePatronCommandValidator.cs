using FluentValidation;
using LibrarySystem.Application.Commands.Patrons;

namespace LibrarySystem.Application.Validators.Patrons;

public class UpdatePatronCommandValidator : AbstractValidator<UpdatePatronCommand>
{
    public UpdatePatronCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("معرف المستعير غير صالح.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب.")
            .MaximumLength(200).WithMessage("الاسم الكامل يجب ألا يتجاوز 200 حرف.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب.")
            .MaximumLength(50).WithMessage("رقم الهاتف يجب ألا يتجاوز 50 حرفاً.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("البريد الإلكتروني غير صالح.")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}
