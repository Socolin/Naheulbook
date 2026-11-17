using AwesomeAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Monster;

[TestFixture]
[TestOf(typeof(MonsterTemplateMapper))]
public class MonsterTemplateMapperTests
{
    private IItemTemplateMapper _itemTemplateMapper;
    private MonsterTemplateMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _itemTemplateMapper = Substitute.For<IItemTemplateMapper>();
        _mapper = new MonsterTemplateMapper(_itemTemplateMapper);
    }

    [Test]
    public void ToResponse_MonsterTemplate_MapsFieldsAndInventory()
    {
        var inventoryElement = new MonsterTemplateInventoryElementEntity {Id = 6, ItemTemplate = new ItemTemplateEntity()};
        var monsterTemplate = new MonsterTemplateEntity
        {
            Id = 3,
            Name = "some-name",
            SubCategoryId = 4,
            Data = """{"at":8}""",
            Items = [inventoryElement],
        };

        var actual = _mapper.ToResponse(monsterTemplate);

        actual.Should().BeEquivalentTo(new MonsterTemplateResponse
        {
            Id = 3,
            Name = "some-name",
            SubCategoryId = 4,
            Data = new JObject {["at"] = 8},
            Inventory = [_mapper.ToResponse(inventoryElement)],
        });
    }

    [Test]
    public void ToResponse_MonsterTemplateWithEmptyData_ReturnsEmptyData()
    {
        var monsterTemplate = new MonsterTemplateEntity {Name = "some-name", Data = "", Items = []};

        var actual = _mapper.ToResponse(monsterTemplate);

        actual.Data.Should().BeEquivalentTo(new JObject());
    }

    [Test]
    public void ToResponse_MonsterTemplateInventoryElement_MapsAllFields()
    {
        var itemTemplate = new ItemTemplateEntity();
        var itemTemplateResponse = new ItemTemplateResponse();
        var inventoryElement = new MonsterTemplateInventoryElementEntity
        {
            Id = 3,
            Chance = 0.5f,
            MinCount = 1,
            MaxCount = 4,
            ItemTemplate = itemTemplate,
        };

        _itemTemplateMapper.ToResponse(itemTemplate)
            .Returns(itemTemplateResponse);

        var actual = _mapper.ToResponse(inventoryElement);

        actual.Should().BeEquivalentTo(new MonsterTemplateResponse.MonsterTemplateInventoryElementResponse
        {
            Id = 3,
            Chance = 0.5f,
            MinCount = 1,
            MaxCount = 4,
            ItemTemplate = itemTemplateResponse,
        });
    }

    [Test]
    public void ToResponse_MonsterType_MapsFieldsAndOrdersSubCategoriesById()
    {
        var subCategory1 = new MonsterSubCategoryEntity {Id = 1, Name = "some-name-1", TypeId = 3};
        var subCategory2 = new MonsterSubCategoryEntity {Id = 2, Name = "some-name-2", TypeId = 3};
        var monsterType = new MonsterTypeEntity
        {
            Id = 3,
            Name = "some-name",
            SubCategories = [subCategory2, subCategory1],
        };

        var actual = _mapper.ToResponse(monsterType);

        actual.Should().BeEquivalentTo(new MonsterTypeResponse
        {
            Id = 3,
            Name = "some-name",
            SubCategories = [_mapper.ToResponse(subCategory1), _mapper.ToResponse(subCategory2)],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_MonsterSubCategory_MapsAllFields()
    {
        var subCategory = new MonsterSubCategoryEntity {Id = 4, Name = "some-name", TypeId = 3};

        var actual = _mapper.ToResponse(subCategory);

        actual.Should().BeEquivalentTo(new MonsterSubCategoryResponse
        {
            Id = 4,
            Name = "some-name",
            TypeId = 3,
        });
    }

    [Test]
    public void ToResponse_MonsterTrait_MapsFieldsAndDeserializeLevels()
    {
        var monsterTrait = new MonsterTraitEntity
        {
            Id = 3,
            Name = "some-name",
            Description = "some-description",
            Levels = """["level-1","level-2"]""",
        };

        var actual = _mapper.ToResponse(monsterTrait);

        actual.Should().BeEquivalentTo(new MonsterTraitResponse
        {
            Id = 3,
            Name = "some-name",
            Description = "some-description",
            Levels = ["level-1", "level-2"],
        });
    }

    [Test]
    public void ToResponse_MonsterTraitWithoutLevels_ReturnsEmptyLevels()
    {
        var monsterTrait = new MonsterTraitEntity {Name = "some-name", Description = "some-description", Levels = null};

        var actual = _mapper.ToResponse(monsterTrait);

        actual.Levels.Should().NotBeNull().And.BeEmpty();
    }
}
