using FluentValidation;
using LibrarySystem.Application.Commands.Users;

namespace LibrarySystem.Application.Validators.Users;

public class CreateSystemUserCommandValidator : AbstractValidator<CreateSystemUserCommand>
{
    public CreateSystemUserCommandValidator()
    {
        RuleFor(x => x.ExternalId).NotEmpty().WithMessage("معرف Identity مطلوب للربط");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200).WithMessage("الاسم الكامل مطلوب");
        RuleFor(x => x.Bio).MaximumLength(1000);
    }
}