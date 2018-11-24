using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class SkillBuilder : BuilderBase<Skill>
    {
        public SkillBuilder WithDefaultTestInfo(int? number = null)
        {
            Entity.Description = $"some-description-{number}";
            Entity.Flags = @"[{""some-flag-data"": ""data""}]";
            Entity.Name = $"some-name-{number}";
            Entity.Require = $"some-require-{number}";
            Entity.PlayerDescription = $"some-player-description-{number}";
            Entity.Resist = $"some-resist-{number}";
            Entity.Roleplay = $"some-roleplay-{number}";
            return this;
        }

        public SkillBuilder WithSkillEffect(string stat = "some-stat", int value = 42)
        {
            Entity.SkillEffects.Add(
                new SkillEffect
                {
                    Value = value,
                    StatName = stat
                });
            return this;
        }
    }
}