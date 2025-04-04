using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators;

public class CreateCharacterRequestValidator: AbstractValidator<CreateCharacterRequest>
{
    private static readonly List<string> ValidSex = ["Homme", "Femme"];
    public CreateCharacterRequestValidator()
    {
        RuleFor(e => e.Sex).NotNull().Must(s => ValidSex.Contains(s));
    }
}