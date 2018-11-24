using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class EffectBuilder : BuilderBase<Effect>
    {
        public EffectBuilder(EffectCategory category)
        {
            Entity.Category = category;
        }

        public EffectBuilder WithDefaultTestInfo()
        {
            Entity.Name = "some-effect-name";
            Entity.Dice = 2;
            Entity.DurationType = "some-duration-type";
            Entity.Duration = "some-duration";
            Entity.Description = "some-description";
            return this;
        }

        public EffectBuilder WithModifier(Stat stat, short value)
        {
            return WithModifier(new EffectModifier()
            {
                Type = "ADD",
                Stat = stat,
                Value = value,
                StatName = stat.Name
            });
        }

        public EffectBuilder WithModifier(EffectModifier modifier)
        {
            Entity.Modifiers.Add(modifier);
            return this;
        }
    }
}