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
                .AddEffectSubCategory()
                .AddEffectSubCategory();

            var actualEffectCategories = await _effectRepository.GetCategoriesAsync();

            actualEffectCategories.Should().BeEquivalentTo(
                TestDataUtil.GetAll<EffectType>(),
                config => config
                    .Excluding(o => o.Id)
                    .IgnoringCyclicReferences());
        }

        [Test]
        public async Task CanGetEffectBySubCategoryWithModifiers()
        {
            TestDataUtil
                .AddEffectType()
                .AddEffectSubCategory()
                .AddStat()
                .AddEffect((effect) => effect.Modifiers = new List<EffectModifier>
                {
                    new EffectModifier
                    {
                        Stat = TestDataUtil.GetLast<StatEntity>(),
                        Type = "ADD",
                        Value = 4
                    }
                });

            var actualEffects = await _effectRepository.GetBySubCategoryWithModifiersAsync(TestDataUtil.Get<EffectSubCategory>().Id);

            actualEffects.Should().BeEquivalentTo(
                TestDataUtil.GetAll<EffectEntity>(),
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(o => o.SubCategory)
                    .Excluding(info => info.Path.EndsWith(".Effect"))
                    .Excluding(info => info.Path.EndsWith(".Stat"))
                    .IgnoringCyclicReferences());
        }

        [Test]
        public async Task CanGetEffectWithModifiers()
        {
            TestDataUtil
                .AddEffectType()
                .AddEffectSubCategory()
                .AddStat()
                .AddEffect((effect) => effect.Modifiers = new List<EffectModifier>
                {
                    new EffectModifier
                    {
                        Stat = TestDataUtil.GetLast<StatEntity>(),
                        Type = "ADD",
                        Value = 4
                    }
                });

            var actualEffect = await _effectRepository.GetWithModifiersAsync(TestDataUtil.Get<EffectEntity>().Id);

            actualEffect.Should().BeEquivalentTo(
                TestDataUtil.Get<EffectEntity>(),
                config => config
                    .Excluding(o => o.Id)
                    .Excluding(o => o.SubCategory)
                    .Excluding(info => info.Path.EndsWith(".Effect"))
                    .Excluding(info => info.Path.EndsWith(".Stat"))
                    .IgnoringCyclicReferences());
        }
    }
}