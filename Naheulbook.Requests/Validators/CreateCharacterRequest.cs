using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateCharacterRequestValidator: AbstractValidator<CreateCharacterRequest>
    {
        public CreateCharacterRequestValidator()
        {
            RuleFor(e => e.Name).NotNull().Length(1, 255);
            RuleFor(e => e.Money).GreaterThanOrEqualTo(0);
            RuleFor(e => e.FatePoint).GreaterThanOrEqualTo((short)0);
            RuleFor(e => e.OriginId).GreaterThanOrEqualTo(0);
        }
    }
}