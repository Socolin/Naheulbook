using AwesomeAssertions;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Effect;

[TestFixture]
[TestOf(typeof(EffectMapper))]
public class EffectMapperTests
{
    private EffectMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new EffectMapper();
    }

    [Test]
    public void ToResponse_Effect_MapsAllFields()
    {
        var effect = new EffectEntity
        {
            Id = 42,
            Name = "some-name",
            Description = "some-description",
            DurationType = "some-duration-type",
            Duration = "some-duration",
            CombatCount = 1,
            LapCount = 2,
            TimeDuration = 3,
            Dice = 4,
            SubCategoryId = 5,
            Modifiers = [CreateModifier("FO", 2)],
        };

        var actual = _mapper.ToResponse(effect);

        actual.Should().BeEquivalentTo(new EffectResponse
        {
            Id = 42,
            Name = "some-name",
            Description = "some-description",
            DurationType = "some-duration-type",
            Duration = "some-duration",
            CombatCount = 1,
            LapCount = 2,
            TimeDuration = 3,
            Dice = 4,
            SubCategoryId = 5,
            Modifiers = [new StatModifierResponse {Stat = "FO", Type = "ADD", Value = 2}],
        });
    }

    [Test]
    public void ToResponse_Effect_OrdersModifiersByStatName()
    {
        var effect = new EffectEntity
        {
            Name = "some-name",
            DurationType = "some-duration-type",
            Modifiers = [CreateModifier("INT", 1), CreateModifier("AD", 2), CreateModifier("FO", 3)],
        };

        var actual = _mapper.ToResponse(effect);

        actual.Modifiers.Select(m => m.Stat).Should().Equal("AD", "FO", "INT");
    }

    [Test]
    public void ToResponse_EffectType_MapsFieldsAndOrdersSubCategoriesById()
    {
        var subCategory1 = CreateSubCategory(1);
        var subCategory2 = CreateSubCategory(2);
        var subCategory3 = CreateSubCategory(3);
        var effectType = new EffectTypeEntity
        {
            Id = 8,
            Name = "some-type-name",
            SubCategories = [subCategory2, subCategory3, subCategory1],
        };

        var actual = _mapper.ToResponse(effectType);

        actual.Should().BeEquivalentTo(new EffectTypeResponse
        {
            Id = 8,
            Name = "some-type-name",
            SubCategories = [_mapper.ToResponse(subCategory1), _mapper.ToResponse(subCategory2), _mapper.ToResponse(subCategory3)],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_EffectSubCategory_MapsAllFields()
    {
        var subCategory = new EffectSubCategoryEntity
        {
            Id = 12,
            Name = "some-name",
            DiceCount = 2,
            DiceSize = 6,
            Note = "some-note",
            TypeId = 3,
        };

        var actual = _mapper.ToResponse(subCategory);

        actual.Should().BeEquivalentTo(new EffectSubCategoryResponse
        {
            Id = 12,
            Name = "some-name",
            DiceCount = 2,
            DiceSize = 6,
            Note = "some-note",
            TypeId = 3,
        });
    }

    [Test]
    public void ToResponse_EffectModifier_MapsStatNameToStat()
    {
        var modifier = CreateModifier("CHA", -3);

        var actual = _mapper.ToResponse(modifier);

        actual.Should().BeEquivalentTo(new StatModifierResponse
        {
            Stat = "CHA",
            Type = "ADD",
            Value = -3,
            Special = null,
        });
    }

    private static EffectModifierEntity CreateModifier(string statName, short value)
    {
        return new EffectModifierEntity
        {
            StatName = statName,
            Type = "ADD",
            Value = value,
        };
    }

    private static EffectSubCategoryEntity CreateSubCategory(int id)
    {
        return new EffectSubCategoryEntity
        {
            Id = id,
            Name = $"some-sub-category-{id}",
            DiceCount = 1,
            DiceSize = 20,
            TypeId = 8,
        };
    }
}
