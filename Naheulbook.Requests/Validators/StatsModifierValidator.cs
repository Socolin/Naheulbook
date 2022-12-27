using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Validators;

public abstract class BaseStatsModifierValidator<T> : AbstractValidator<T>
    where T : StatsModifier
{
    private static List<string> ValidDurationTypes => new List<string> {"combat", "custom", "time", "lap", "forever"};

    protected BaseStatsModifierValidator()
    {
        RuleFor(x => x.Name).NotEmpty().NotNull().MaximumLength(255);
        RuleFor(r => r.DurationType).Must(s => ValidDurationTypes.Contains(s));
        RuleForEach(x => x.Values).SetValidator(new StatModifierValidator());
    }
}

public class StatsModifierValidator : BaseStatsModifierValidator<StatsModifier>
{
}