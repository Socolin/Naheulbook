using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.Tests.Integration.EntityBuilders;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class EffectRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private EffectRepository _effectRepository;

        [SetUp]
        public void SetUp()
        {
            _effectRepository = new EffectRepository(RepositoryDbContext);
        }

        [Test]
        public async Task CanGetEffectCategories()
        {
            var effectType = new EffectTypeBuilder().WithDefaultTestInfo().Build();
            var effectTypes = await AddInDbAsync(effectType);

            var effectCategory1 = new EffectCategoryBuilder(effectType).WithDefaultTestInfo().Build();
            var effectCategory2 = new EffectCategoryBuilder(effectType).WithDefaultTestInfo().Build();
            await AddInDbAsync(effectCategory1, effectCategory2);

            var actualEffectCategories = await _effectRepository.GetCategoriesAsync();

            actualEffectCategories.Should().BeEquivalentTo(
                effectTypes,
                config => config
                    .Excluding(o => o.Id)
                    .IgnoringCyclicReferences());
        }

        [Test]
        public async Task CanGetEffectByCategoryWithModifiers()
        {
            var effectCategory = await CreateEffectCategoryInDb();
            var effects = await CreateEffectsInDb(effectCategory);

            var actualEffects = await _effectRepository.GetByCategoryWithModifiersAsync(effectCategory.Id);

            actualEffects.Should().BeEquivalentTo(
                effects,
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(o => o.Category)
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Effect"))
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Stat"))
                    .IgnoringCyclicReferences());
        }

        [Test]
        public async Task CanGetEffectWithModifiers()
        {
            var effectCategory = await CreateEffectCategoryInDb();
            var effects = await CreateEffectsInDb(effectCategory);

            var actualEffect = await _effectRepository.GetWithModifiersAsync(effects[0].Id);

            actualEffect.Should().BeEquivalentTo(
                effects[0],
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(o => o.Category)
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Effect"))
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Stat"))
                    .IgnoringCyclicReferences());
        }

        private async Task<EffectCategory> CreateEffectCategoryInDb()
        {
            var effectType = new EffectTypeBuilder().WithDefaultTestInfo().Build();
            await AddInDbAsync(effectType);

            var effectCategory = new EffectCategoryBuilder(effectType).WithDefaultTestInfo().Build();
            await AddInDbAsync(effectCategory);

            return effectCategory;
        }

        private async Task<List<Effect>> CreateEffectsInDb(EffectCategory effectCategory)
        {
            var stat1 = new StatBuilder().WithDefaultTestInfo("some-stat-1").Build();
            var stat2 = new StatBuilder().WithDefaultTestInfo("some-stat-2").Build();
            await AddInDbAsync(stat1, stat2);

            var effect = new EffectBuilder(effectCategory)
                .WithDefaultTestInfo()
                .WithModifier(stat1, 5)
                .WithModifier(stat2, -4)
                .Build();
            return await AddInDbAsync(effect);
        }
    }
}