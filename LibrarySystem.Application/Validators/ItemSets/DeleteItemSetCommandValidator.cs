using FluentValidation;
using LibrarySystem.Application.Commands.ItemSets;

namespace LibrarySystem.Application.Validators.ItemSets;

public class DeleteItemSetCommandValidator : AbstractValidator<DeleteItemSetCommand>
{
    public DeleteItemSetCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
