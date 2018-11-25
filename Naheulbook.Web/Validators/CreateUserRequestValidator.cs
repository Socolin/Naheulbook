using FluentValidation;
using Naheulbook.Web.Requests;

namespace Naheulbook.Web.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(request => request.Username).NotNull().EmailAddress();
            RuleFor(request => request.Password).NotNull().MinimumLength(64);
        }
    }
}