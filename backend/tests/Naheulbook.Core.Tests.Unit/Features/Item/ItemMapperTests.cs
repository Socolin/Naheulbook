using AwesomeAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Item;

[TestFixture]
[TestOf(typeof(ItemMapper))]
public class ItemMapperTests
{
    private const string SomeModifiersJson = """[{"name":"some-modifier","durationType":"forever","values":[{"stat":"AD","type":"ADD","value":2}],"active":true}]""";

    private IItemTemplateMapper _itemTemplateMapper;
    private ItemMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _itemTemplateMapper = Substitute.For<IItemTemplateMapper>();
        _mapper = new ItemMapper(_itemTemplateMapper);
    }

    [Test]
    public void ToResponse_Item_MapsAllFields()
    {
        var itemTemplate = new ItemTemplateEntity();
        var itemTemplateResponse = new ItemTemplateResponse();
        var item = new ItemEntity
        {
            Id = 3,
            Data = """{"name":"some-name"}""",
            Modifiers = SomeModifiersJson,
            ContainerId = 4,
            ItemTemplate = itemTemplate,
        };

        _itemTemplateMapper.ToResponse(itemTemplate)
            .Returns(itemTemplateResponse);

        var actual = _mapper.ToResponse(item);

        actual.Should().BeEquivalentTo(new ItemResponse
        {
            Id = 3,
            Data = new JObject {["name"] = "some-name"},
            Modifiers = [CreateExpectedModifier()],
            ContainerId = 4,
            ItemTemplate = itemTemplateResponse,
        });
    }

    [Test]
    public void ToResponse_ItemWithoutDataAndModifiers_ReturnsEmptyDataAndModifiers()
    {
        var item = new ItemEntity
        {
            Data = "",
            Modifiers = null,
            ItemTemplate = new ItemTemplateEntity(),
        };

        var actual = _mapper.ToResponse(item);

        actual.Data.Should().BeEquivalentTo(new JObject());
        actual.Modifiers.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToPartialResponse_Item_MapsAllFieldsButItemTemplate()
    {
        var item = new ItemEntity
        {
            Id = 3,
            Data = """{"name":"some-name"}""",
            Modifiers = SomeModifiersJson,
            ContainerId = 4,
        };

        var actual = _mapper.ToPartialResponse(item);

        actual.Should().BeEquivalentTo(new ItemPartialResponse
        {
            Id = 3,
            Data = new JObject {["name"] = "some-name"},
            Modifiers = [CreateExpectedModifier()],
            ContainerId = 4,
        });
    }

    [Test]
    public void ToPartialResponse_ItemWithoutDataAndModifiers_ReturnsEmptyDataAndModifiers()
    {
        var item = new ItemEntity {Data = "", Modifiers = null};

        var actual = _mapper.ToPartialResponse(item);

        actual.Data.Should().BeEquivalentTo(new JObject());
        actual.Modifiers.Should().NotBeNull().And.BeEmpty();
    }

    private static ActiveStatsModifier CreateExpectedModifier()
    {
        return new ActiveStatsModifier
        {
            Name = "some-modifier",
            DurationType = "forever",
            Active = true,
            Values = [new StatModifierRequest {Stat = "AD", Type = "ADD", Value = 2}],
        };
    }
}
