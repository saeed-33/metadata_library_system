using FluentValidation;
using LibrarySystem.Application.Commands.Users;

namespace LibrarySystem.Application.Validators.Users;

public class UpdateSystemUserCommandValidator : AbstractValidator<UpdateSystemUserCommand>
{
    public UpdateSystemUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("معرف المستخدم مطلوب")
            .GreaterThan(0).WithMessage("معرف المستخدم يجب أن يكون رقماً صحيحاً");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب للتعديل")
            .MaximumLength(200).WithMessage("الاسم لا يمكن أن يتجاوز 200 حرف");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("نبذة المستخدم طويلة جداً");
    }
}