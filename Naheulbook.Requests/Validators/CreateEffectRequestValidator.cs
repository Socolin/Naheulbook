using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateEffectRequestValidator : AbstractValidator<CreateEffectRequest>
    {
        public CreateEffectRequestValidator()
        {
            RuleFor(e => e.Name).NotNull().Length(1, 255);
            RuleFor(e => e.CategoryId).GreaterThan(0);
            RuleFor(e => e.Dice).GreaterThanOrEqualTo((short) 0);
        }
    }
}