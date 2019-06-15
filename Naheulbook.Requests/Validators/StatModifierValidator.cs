using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Validators
{
    public class StatModifierValidator : AbstractValidator<StatModifier>
    {
        private static readonly List<string> ValidTypeValues = new List<string> {"ADD", "MUL", "DIV", "SET", "PERCENTAGE"};

        public StatModifierValidator()
        {
            RuleFor(r => r.Stat).NotNull().NotEmpty();
            RuleFor(r => r.Type).Must(s => ValidTypeValues.Contains(s));
        }
    }
}