using FluentValidation;
using Naheulbook.Web.Requests;

namespace Naheulbook.Web.Validators
{
    public class GenerateJwtRequestValidator : AbstractValidator<GenerateJwtRequest>
    {
        public GenerateJwtRequestValidator()
        {
            RuleFor(request => request.Password).NotNull().MinimumLength(64);
        }
    }
}