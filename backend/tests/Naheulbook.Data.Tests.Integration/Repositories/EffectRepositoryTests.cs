using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class EffectRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private EffectRepository _effectRepository;

    [SetUp]
    public void SetUp()
    {
        _effectRepository = new EffectRepository(RepositoryDbContext);
    }

    #region GetCategoriesAsync

    [Test]
    public async Task CanGetEffectCategories()
    {
        TestDataUtil
            .AddEffectType(out var effectCategory)
            .AddEffectSubCategory(out var effectSubCategory1)
            .AddEffectSubCategory(out var effectSubCategory2);

        var actualEffectCategories = await _effectRepository.GetCategoriesAsync();

        AssertEntitiesAreLoaded(actualEffectCategories, new[] {effectCategory});
        AssertEntitiesAreLoaded(actualEffectCategories.Single().SubCategories, new[] {effectSubCategory1, effectSubCategory2});
    }

    #endregion

    #region GetBySubCategoryWithModifiersAsync

    [Test]
    public async Task CanGetEffectBySubCategoryWithModifiers()
    {
        TestDataUtil
            .AddEffectType()
            .AddEffectSubCategory(out var effectSubCategory)
            .AddEffect(out var effect)
            .AddStat()
            .AddEffectModifier(out var effectModifier);

        var actualEffects = await _effectRepository.GetBySubCategoryWithModifiersAsync(effectSubCategory.Id);

        AssertEntitiesAreLoaded(actualEffects, new[] {effect});
        AssertEntitiesAreLoaded(actualEffects.Single().Modifiers, new[] {effectModifier});
    }

    #endregion

    #region GetWithModifiersAsync

    [Test]
    public async Task CanGetEffectWithModifiers()
    {
        TestDataUtil
            .AddEffectType()
            .AddEffectSubCategory()
            .AddStat()
            .AddEffect((effect) => effect.Modifiers = new List<EffectModifierEntity>
            {
                new()
                {
                    Stat = TestDataUtil.GetLast<StatEntity>(),
                    Type = "ADD",
                    Value = 4,
                },
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

    #endregion

    #region SearchByNameAsync

    #endregion

    #region GetWithEffectWithModifiersAsync

    #endregion
}