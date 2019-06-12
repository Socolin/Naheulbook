using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
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
            TestDataUtil
                .AddEffectType()
                .AddEffectCategory()
                .AddEffectCategory();

            var actualEffectCategories = await _effectRepository.GetCategoriesAsync();

            actualEffectCategories.Should().BeEquivalentTo(
                TestDataUtil.GetAll<EffectType>(),
                config => config
                    .Excluding(o => o.Id)
                    .IgnoringCyclicReferences());
        }

        [Test]
        public async Task CanGetEffectByCategoryWithModifiers()
        {
            TestDataUtil
                .AddEffectType()
                .AddEffectCategory()
                .AddStat()
                .AddEffect((effect) => effect.Modifiers = new List<EffectModifier>
                {
                    new EffectModifier
                    {
                        Stat = TestDataUtil.GetLast<Stat>(),
                        Type = "ADD",
                        Value = 4
                    }
                });

            var actualEffects = await _effectRepository.GetByCategoryWithModifiersAsync(TestDataUtil.Get<EffectCategory>().Id);

            actualEffects.Should().BeEquivalentTo(
                TestDataUtil.GetAll<Effect>(),
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
            TestDataUtil
                .AddEffectType()
                .AddEffectCategory()
                .AddStat()
                .AddEffect((effect) => effect.Modifiers = new List<EffectModifier>
                {
                    new EffectModifier
                    {
                        Stat = TestDataUtil.GetLast<Stat>(),
                        Type = "ADD",
                        Value = 4
                    }
                });

            var actualEffect = await _effectRepository.GetWithModifiersAsync(TestDataUtil.Get<Effect>().Id);

            actualEffect.Should().BeEquivalentTo(
                TestDataUtil.Get<Effect>(),
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(o => o.Category)
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Effect"))
                    .Excluding(info => info.SelectedMemberPath.EndsWith(".Stat"))
                    .IgnoringCyclicReferences());
        }
    }
}