using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateCharacterRequestValidator: AbstractValidator<CreateCharacterRequest>
    {
        private static readonly List<string> ValidSex = new List<string> {"Homme", "Femme"};
        public CreateCharacterRequestValidator()
        {
            RuleFor(e => e.Name).NotNull().Length(1, 255);
            RuleFor(e => e.Sex).NotNull().Must(s => ValidSex.Contains(s));
            RuleFor(e => e.Notes).Length(0, 20_000);
            RuleFor(e => e.Money).GreaterThanOrEqualTo(0);
            RuleFor(e => e.FatePoint).GreaterThanOrEqualTo((short)0);
        }
    }
}