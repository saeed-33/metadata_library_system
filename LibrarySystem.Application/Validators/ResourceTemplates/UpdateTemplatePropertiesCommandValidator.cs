using FluentValidation;
using LibrarySystem.Application.Commands.ResourceTemplates;

namespace LibrarySystem.Application.Validators.ResourceTemplates;

public class UpdateTemplatePropertiesCommandValidator : AbstractValidator<UpdateTemplatePropertiesCommand>
{
    public UpdateTemplatePropertiesCommandValidator()
    {
        RuleFor(x => x.TemplateId).GreaterThan(0).WithMessage("معرف القالب غير صحيح");

        RuleFor(x => x.Properties).NotEmpty().WithMessage("يجب إضافة خاصية واحدة على الأقل للقالب");

        RuleForEach(x => x.Properties).ChildRules(p => {
            p.RuleFor(x => x.PropertyId).GreaterThan(0).WithMessage("معرف الخاصية غير صحيح");
            p.RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        });
    }
}