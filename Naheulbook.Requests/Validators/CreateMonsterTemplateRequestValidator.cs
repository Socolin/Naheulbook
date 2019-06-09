using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateMonsterTemplateRequestValidator : AbstractValidator<CreateMonsterTemplateRequest>
    {
        public CreateMonsterTemplateRequestValidator()
        {
            RuleFor(x => x.CategoryId).GreaterThan(0).NotNull();
            RuleFor(x => x.Monster).NotNull();
            RuleFor(x => x.Monster.Name).NotEmpty().NotNull();
            RuleFor(x => x.Monster.Data).NotNull();
        }
    }
}