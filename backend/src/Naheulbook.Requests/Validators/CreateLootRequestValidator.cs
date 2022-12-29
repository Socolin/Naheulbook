using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators;

public class CreateLootRequestValidator : AbstractValidator<CreateLootRequest>
{
    public CreateLootRequestValidator()
    {
        RuleFor(e => e.Name).NotNull().Length(1, 255);
    }
}