using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Validators
{
    public class LapCountDecrementValidator : AbstractValidator<LapCountDecrement>
    {
        private static readonly List<string> ValidWhenValues = new List<string> {"BEFORE", "AFTER"};

        public LapCountDecrementValidator()
        {
            RuleFor(r => r.When).Must(s => ValidWhenValues.Contains(s));
            RuleFor(r => r.FighterId).GreaterThan(0);
        }
    }
}