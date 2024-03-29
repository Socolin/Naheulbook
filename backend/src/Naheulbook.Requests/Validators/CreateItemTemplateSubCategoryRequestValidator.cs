using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators;

public class CreateItemTemplateSubCategoryRequestValidator : AbstractValidator<CreateItemTemplateSubCategoryRequest>
{
    public CreateItemTemplateSubCategoryRequestValidator()
    {
        RuleFor(i => i.Name).NotNull().Length(1, 255);
        RuleFor(i => i.Description).NotNull().Length(0, 255);
        RuleFor(i => i.Note).NotNull();
        RuleFor(i => i.TechName).NotNull().Length(1, 255);
        RuleFor(i => i.SectionId).GreaterThan(0);
    }
}