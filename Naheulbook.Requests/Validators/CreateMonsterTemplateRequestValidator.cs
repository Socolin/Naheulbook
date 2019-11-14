using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateMonsterTemplateRequestValidator : AbstractValidator<MonsterTemplateRequest>
    {
        public CreateMonsterTemplateRequestValidator()
        {
            RuleFor(x => x.SubCategoryId).GreaterThan(0).NotNull();
            RuleFor(x => x.Name).NotEmpty().NotNull();
            RuleFor(x => x.Data).NotNull();
        }
    }
}