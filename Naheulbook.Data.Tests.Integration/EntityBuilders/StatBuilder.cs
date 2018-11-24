using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class StatBuilder : BuilderBase<Stat>
    {
        public StatBuilder WithDefaultTestInfo(string name = "some-stat-name")
        {
            return WithDescription("some-description")
                .WithBonus("some-bonus")
                .WithDisplayName("some-display-name")
                .WithPenalty("some-penalty")
                .WithName(name);
        }

        public StatBuilder WithName(string name)
        {
            Entity.Name = name;
            return this;
        }

        public StatBuilder WithDisplayName(string displayName)
        {
            Entity.DisplayName = displayName;
            return this;
        }

        public StatBuilder WithDescription(string description)
        {
            Entity.Description = description;
            return this;
        }

        public StatBuilder WithBonus(string bonus)
        {
            Entity.Bonus = bonus;
            return this;
        }

        public StatBuilder WithPenalty(string penalty)
        {
            Entity.Penalty = penalty;
            return this;
        }
    }
}