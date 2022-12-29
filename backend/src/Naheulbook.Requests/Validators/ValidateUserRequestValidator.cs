using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators;

public class ValidateUserRequestValidator : AbstractValidator<ValidateUserRequest>
{
    public ValidateUserRequestValidator()
    {
        RuleFor(request => request.ActivationCode).NotNull().NotEmpty();
    }
}