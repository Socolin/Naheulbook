using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class CreateItemTemplateRequestValidator : AbstractValidator<CreateItemTemplateRequest>
    {
        private static readonly List<string> ValidSources = new List<string> {"official", "private", "community"};

        public CreateItemTemplateRequestValidator()
        {
            RuleFor(r => r.Name).NotEmpty().Length(1, 255);
            RuleFor(r => r.Source).Must(s => ValidSources.Contains(s));
            RuleFor(r => r.CategoryId).GreaterThan(0);
            RuleFor(r => r.Data).NotNull();
            RuleFor(r => r.Modifiers).NotNull();
            RuleFor(r => r.Skills).NotNull();
            RuleFor(r => r.UnSkills).NotNull();
            RuleFor(r => r.SkillModifiers).NotNull();
            RuleFor(r => r.Requirements).NotNull();
            RuleFor(r => r.Slots).NotNull();
        }
    }

    public class CreateItemTemplateModifierRequestValidator : AbstractValidator<CreateItemTemplateModifierRequest>
    {
        public CreateItemTemplateModifierRequestValidator()
        {
            RuleFor(r => r.Type).NotNull().NotEmpty();
            RuleFor(r => r.Stat).NotNull().NotEmpty();
        }
    }
    public class CreateItemTemplateSkillModifierRequestValidator : AbstractValidator<CreateItemTemplateSkillModifierRequest>
    {
        public CreateItemTemplateSkillModifierRequestValidator()
        {
            RuleFor(r => r.Skill).GreaterThan(0);
        }
    }

    public class CreateItemTemplateRequirementRequestValidator : AbstractValidator<CreateItemTemplateRequirementRequest>
    {
        public CreateItemTemplateRequirementRequestValidator()
        {
            RuleFor(r => r.Stat).NotNull().NotEmpty();
        }
    }
}