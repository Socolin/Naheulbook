using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators;

public class CreateEffectRequestValidator : AbstractValidator<CreateEffectRequest>
{
    public CreateEffectRequestValidator()
    {
        RuleFor(e => e.Dice).GreaterThanOrEqualTo((short) 0);
        RuleForEach(e => e.Modifiers).SetValidator(new StatModifierValidator());
    }
}