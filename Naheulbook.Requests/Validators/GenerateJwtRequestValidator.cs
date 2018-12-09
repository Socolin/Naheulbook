using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class GenerateJwtRequestValidator : AbstractValidator<GenerateJwtRequest>
    {
        public GenerateJwtRequestValidator()
        {
            RuleFor(request => request.Password).NotNull().MinimumLength(64);
        }
    }
}