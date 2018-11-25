using FluentValidation;
using Naheulbook.Web.Requests;

namespace Naheulbook.Web.Validators
{
    public class ValidateUserRequestValidator : AbstractValidator<ValidateUserRequest>
    {
        public ValidateUserRequestValidator()
        {
            RuleFor(request => request.ActivationCode).NotNull().NotEmpty();
        }
    }
}