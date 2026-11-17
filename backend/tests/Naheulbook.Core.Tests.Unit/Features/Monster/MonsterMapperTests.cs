using AwesomeAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Monster;

[TestFixture]
[TestOf(typeof(MonsterMapper))]
public class MonsterMapperTests
{
    private IItemMapper _itemMapper;
    private MonsterMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _itemMapper = Substitute.For<IItemMapper>();
        _mapper = new MonsterMapper(_itemMapper);
    }

    [Test]
    public void ToResponse_Monster_MapsAllFields()
    {
        var item = new ItemEntity();
        var itemResponse = new ItemResponse();
        var monster = new MonsterEntity
        {
            Id = 3,
            FightId = 4,
            Name = "some-name",
            Dead = new DateTimeOffset(2024, 3, 4, 5, 6, 7, TimeSpan.Zero),
            Data = """{"at":8}""",
            Modifiers = """[{"name":"some-modifier","durationType":"forever","values":[]}]""",
            Items = [item],
        };

        _itemMapper.ToResponse(item)
            .Returns(itemResponse);

        var actual = _mapper.ToResponse(monster);

        actual.Should().BeEquivalentTo(new MonsterResponse
        {
            Id = 3,
            FightId = 4,
            Name = "some-name",
            Dead = "2024-03-04T05:06:07",
            Data = new JObject {["at"] = 8},
            Modifiers = [new ActiveStatsModifier {Name = "some-modifier", DurationType = "forever", Values = []}],
            Items = [itemResponse],
        });
    }

    [Test]
    public void ToResponse_AliveMonsterWithoutDataAndModifiers_ReturnsNullDeadAndEmptyDataAndModifiers()
    {
        var monster = new MonsterEntity
        {
            Name = "some-name",
            Dead = null,
            Data = null,
            Modifiers = null,
            Items = [],
        };

        var actual = _mapper.ToResponse(monster);

        actual.Dead.Should().BeNull();
        actual.Data.Should().BeEquivalentTo(new JObject());
        actual.Modifiers.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToDeadResponse_Monster_MapsIdNameDeadAndData()
    {
        var monster = new MonsterEntity
        {
            Id = 3,
            Name = "some-name",
            Dead = new DateTimeOffset(2024, 3, 4, 5, 6, 7, TimeSpan.Zero),
            Data = """{"at":8}""",
        };

        var actual = _mapper.ToDeadResponse(monster);

        actual.Should().BeEquivalentTo(new DeadMonsterResponse
        {
            Id = 3,
            Name = "some-name",
            Dead = "2024-03-04T05:06:07",
            Data = new JObject {["at"] = 8},
        });
    }

    [Test]
    public void ToDeadResponse_MonsterWithoutData_ReturnsEmptyData()
    {
        var monster = new MonsterEntity {Name = "some-name", Data = null};

        var actual = _mapper.ToDeadResponse(monster);

        actual.Data.Should().BeEquivalentTo(new JObject());
    }
}
