using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class JobBuilder : BuilderBase<Job>
    {
        public JobBuilder WithDefaultTestInfo()
        {
            Entity.Name = "some-name";
            Entity.InternalName = "some-internal-name";
            Entity.Information = "some-information";
            Entity.PlayerDescription = "some-player-description";
            Entity.PlayerSummary = "some-player-summary";
            Entity.Flags = "[]";
            Entity.MaxLoad = 42;
            Entity.IsMagic = true;
            Entity.MaxArmorPr = 25;
            Entity.BaseEv = 32;
            Entity.BaseEa = 12;
            return this;
        }

        public JobBuilder WithBonus(string description = "some-description", string flags = "[]")
        {
            var bonus = new JobBonus
            {
                Description = description,
                Flags = flags
            };
            Entity.Bonuses.Add(bonus);
            return this;
        }

        public JobBuilder WithOriginBlacklist(Origin origin)
        {
            var blacklist = new JobOriginBlacklist
            {
                Origin = origin
            };
            Entity.OriginBlacklist.Add(blacklist);
            return this;
        }

        public JobBuilder WithOriginWhitelist(Origin origin)
        {
            var whitelist = new JobOriginWhitelist
            {
                Origin = origin
            };
            Entity.OriginWhitelist.Add(whitelist);
            return this;
        }

        public JobBuilder WithRequirement(Stat stat, long? min, long? max)
        {
            var requirement = new JobRequirement
            {
                Stat = stat,
                MinValue = min,
                MaxValue = max
            };
            Entity.Requirements.Add(requirement);
            return this;
        }

        public JobBuilder WithRestriction(string text = "some-text", string flags = "[]")
        {
            var restriction = new JobRestrict
            {
                Text = text,
                Flags = flags
            };
            Entity.Restrictions.Add(restriction);
            return this;
        }

        public JobBuilder WithDefaultSkill(Skill skill)
        {
            var jobSkill = new JobSkill
            {
                Skill = skill,
                SkillId = skill.Id,
                Default = true
            };
            Entity.Skills.Add(jobSkill);
            return this;
        }

        public JobBuilder WithAvailableSkill(Skill skill)
        {
            var jobSkill = new JobSkill
            {
                Skill = skill,
                SkillId = skill.Id,
                Default = false
            };
            Entity.Skills.Add(jobSkill);
            return this;
        }

        public JobBuilder WithSpecialities(Speciality speciality)
        {
            Entity.Specialities.Add(speciality);
            return this;
        }
    }
}