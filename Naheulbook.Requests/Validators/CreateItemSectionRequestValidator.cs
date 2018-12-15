using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateItemSectionRequestValidator : AbstractValidator<CreateItemTemplateSectionRequest>
    {
        public CreateItemSectionRequestValidator()
        {
            RuleFor(e => e.Name).NotNull().Length(1, 255);
        }
    }
}