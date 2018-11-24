using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class OriginBuilder : BuilderBase<Origin>
    {
        public OriginBuilder WithDefaultTestInfo()
        {
            Entity.Name = "some-name";
            Entity.Description = "some-description";
            Entity.PlayerDescription = "some-player-description";
            Entity.PlayerSummary = "some-player-summary";
            Entity.Advantage = "some-advantage";
            Entity.Size = "some-size";
            Entity.Flags = "[]";
            Entity.MaxLoad = 42;
            Entity.MaxArmorPr = 25;
            Entity.BaseEv = 32;
            Entity.BaseEa = 12;
            Entity.BonusAt = 6;
            Entity.BonusPrd = 5;
            Entity.DiceEvLevelUp = 1;
            Entity.SpeedModifier = 1;
            return this;
        }

        public OriginBuilder WithName(string name)
        {
            Entity.Name = name;
            return this;
        }

        public OriginBuilder WithInfo(string title = "some-title", string description = "some-description")
        {
            Entity.Information.Add(new OriginInfo
            {
                Description = description,
                Title = title
            });

            return this;
        }

        public OriginBuilder WithRequirement(Stat stat, int? minValue = 5, int? maxValue = 10)
        {
            Entity.Requirements.Add(new OriginRequirement
            {
                StatName = stat.Name,
                Stat = stat,
                MinValue = minValue,
                MaxValue = maxValue
            });

            return this;
        }

        public OriginBuilder WithRequirement(string stat, int? minValue = 5, int? maxValue = 10)
        {
            Entity.Requirements.Add(new OriginRequirement
            {
                StatName = stat,
                MinValue = minValue,
                MaxValue = maxValue
            });

            return this;
        }

        public OriginBuilder WithBonus(string flags = "[]", string description = "some-description")
        {
            Entity.Bonuses.Add(new OriginBonus
            {
                Flags = flags,
                Description = description
            });
            return this;
        }

        public OriginBuilder WithDefaultSkill(Skill skill)
        {
            Entity.Skills.Add(new OriginSkill
            {
                Skill = skill,
                SkillId = skill.Id,
                Default = true
            });

            return this;
        }

        public OriginBuilder WithAvailableSkill(Skill skill)
        {
            Entity.Skills.Add(new OriginSkill
            {
                Skill = skill,
                SkillId = skill.Id,
                Default = false
            });

            return this;
        }
    }
}