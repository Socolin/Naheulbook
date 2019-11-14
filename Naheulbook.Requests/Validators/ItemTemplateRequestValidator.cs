using System.Collections.Generic;
using FluentValidation;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Requests.Validators
{
    public class ItemTemplateRequestValidator : AbstractValidator<ItemTemplateRequest>
    {
        private static readonly List<string> ValidSources = new List<string> {"official", "private", "community"};

        public ItemTemplateRequestValidator()
        {
            RuleFor(r => r.Name).NotEmpty().Length(1, 255);
            RuleFor(r => r.Source).Must(s => ValidSources.Contains(s));
            RuleFor(r => r.SubCategoryId).GreaterThan(0);
            RuleFor(r => r.Data).NotNull();
        }
    }

    public class ItemTemplateModifierRequestValidator : AbstractValidator<ItemTemplateModifierRequest>
    {
        public ItemTemplateModifierRequestValidator()
        {
            RuleFor(r => r.Type).NotNull().NotEmpty();
            RuleFor(r => r.Stat).NotNull().NotEmpty();
        }
    }

    public class ItemTemplateSkillModifierRequestValidator : AbstractValidator<ItemTemplateSkillModifierRequest>
    {
        public ItemTemplateSkillModifierRequestValidator()
        {
            RuleFor(r => r.Skill).GreaterThan(0);
        }
    }

    public class ItemTemplateRequirementRequestValidator : AbstractValidator<ItemTemplateRequirementRequest>
    {
        public ItemTemplateRequirementRequestValidator()
        {
            RuleFor(r => r.Stat).NotNull().NotEmpty();
        }
    }
}