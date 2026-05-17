using FluentValidation;
using LibrarySystem.Application.Commands.ItemSets;

namespace LibrarySystem.Application.Validators.ItemSets;

public class AddItemToItemSetCommandValidator : AbstractValidator<AddItemToItemSetCommand>
{
    public AddItemToItemSetCommandValidator()
    {
        RuleFor(x => x.ItemSetId).GreaterThan(0);
        RuleFor(x => x.ItemId).GreaterThan(0);
    }
}
