using FluentValidation;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Validators;

public class StatModifierValidator : AbstractValidator<StatModifierRequest>
{
    private static readonly List<string> ValidTypeValues = ["ADD", "MUL", "DIV", "SET", "PERCENTAGE"];

    public StatModifierValidator()
    {
        RuleFor(r => r.Stat).NotNull().NotEmpty();
        RuleFor(r => r.Type).Must(s => ValidTypeValues.Contains(s));
    }
}