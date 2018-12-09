using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
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