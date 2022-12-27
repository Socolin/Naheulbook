using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators;

public class CreateEffectTypeRequestValidator : AbstractValidator<CreateEffectTypeRequest>
{
    public CreateEffectTypeRequestValidator()
    {
        RuleFor(e => e.Name).NotNull().Length(1, 255);
    }
}