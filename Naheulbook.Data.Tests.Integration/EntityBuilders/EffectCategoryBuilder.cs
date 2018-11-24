using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class EffectCategoryBuilder : BuilderBase<EffectCategory>
    {
        public EffectCategoryBuilder(EffectType effectType)
        {
            Entity.Type = effectType;
        }

        public EffectCategoryBuilder WithDefaultTestInfo()
        {
            Entity.Name = "some-type-name";
            Entity.Note = "some-note";
            Entity.DiceSize = 6;
            Entity.DiceCount = 2;
            return this;
        }

        public EffectCategoryBuilder WithEffect(Effect effect)
        {
            Entity.Effects.Add(effect);
            return this;
        }
    }
}