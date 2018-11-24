using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class EffectTypeBuilder : BuilderBase<EffectType>
    {
        public EffectTypeBuilder WithDefaultTestInfo()
        {
            Entity.Name = "some-effect-type-name";
            return this;
        }

        public EffectTypeBuilder WithCategory(EffectCategory category)
        {
            Entity.Categories.Add(category);
            return this;
        }
    }
}