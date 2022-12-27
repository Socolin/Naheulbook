using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators;

public class CharacterLevelUpRequestValidator : AbstractValidator<CharacterLevelUpRequest>
{
    private static readonly List<string> ValidEvEa = new List<string> {"EV", "EA"};
    private static readonly List<string> ValidStats = new List<string> {"AD", "FO", "COU", "INT", "PRD", "AT"};

    public CharacterLevelUpRequestValidator()
    {
        RuleFor(r => r.EvOrEa).Must(s => ValidEvEa.Contains(s));
        RuleFor(r => r.StatToUp).Must(s => ValidStats.Contains(s));
        RuleFor(r => r.SpecialityIds).NotNull();
    }
}