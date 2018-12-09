using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateEffectCategoryRequestValidator : AbstractValidator<CreateEffectCategoryRequest>
    {
        public CreateEffectCategoryRequestValidator()
        {
            RuleFor(e => e.Name).NotNull().Length(1, 255);
            RuleFor(e => e.DiceSize).GreaterThanOrEqualTo((short) 0);
            RuleFor(e => e.DiceCount).GreaterThanOrEqualTo((short) 0);
            RuleFor(e => e.Note).MaximumLength(10000);
            RuleFor(e => e.TypeId).GreaterThan(0);
        }
    }
}