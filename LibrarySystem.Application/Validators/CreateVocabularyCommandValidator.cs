using FluentValidation;

using LibrarySystem.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Validators
{
    public class CreateVocabularyCommandValidator : AbstractValidator<CreateVocabularyCommand>
    {
        public CreateVocabularyCommandValidator()
        {
            RuleFor(x => x.Prefix)
                .NotEmpty().WithMessage("Prefix is required.")
                .Length(1,50).WithMessage("Prefix cannot exceed 50 characters.");

            RuleFor(x => x.NamespaceUri)
                .NotEmpty().WithMessage("Namespace URI is required.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Namespace URI must be a valid URL.");

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Label is required.")
                .Length(1,100).WithMessage("Label cannot exceed 100 characters.");
        }
    }
}
