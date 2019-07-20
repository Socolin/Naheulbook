using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateEffectRequestValidator : AbstractValidator<CreateEffectRequest>
    {
        public CreateEffectRequestValidator()
        {
            RuleFor(e => e.Name).NotNull().Length(1, 255);
            RuleFor(e => e.Modifiers).NotNull();
            RuleFor(e => e.Dice).GreaterThanOrEqualTo((short) 0);
            RuleForEach(e => e.Modifiers).SetValidator(new StatModifierValidator());
        }
    }
}