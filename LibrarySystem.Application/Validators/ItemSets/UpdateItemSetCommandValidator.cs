using FluentValidation;
using LibrarySystem.Application.Commands.ItemSets;

namespace LibrarySystem.Application.Validators.ItemSets;

public class UpdateItemSetCommandValidator : AbstractValidator<UpdateItemSetCommand>
{
    public UpdateItemSetCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("اسم المجموعة مطلوب")
            .MaximumLength(200).WithMessage("اسم المجموعة طويل جداً");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("الوصف طويل جداً");

        RuleFor(x => x.OwnerId)
            .GreaterThan(0).When(x => x.OwnerId.HasValue);
    }
}
