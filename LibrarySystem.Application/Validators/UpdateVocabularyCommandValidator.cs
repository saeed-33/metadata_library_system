using FluentValidation;
using LibrarySystem.Application.Commands;
namespace LibrarySystem.Application.Validators
{
    public class UpdateVocabularyCommandValidator : AbstractValidator<UpdateVocabularyCommand>
    {
        public UpdateVocabularyCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Vocabulary ID.");

            RuleFor(x => x.Prefix)
                .NotEmpty().WithMessage("Prefix is required.");

            RuleFor(x => x.NamespaceUri)
                .NotEmpty().WithMessage("Namespace URI is required.");

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Label is required.");
        }
    }
}

