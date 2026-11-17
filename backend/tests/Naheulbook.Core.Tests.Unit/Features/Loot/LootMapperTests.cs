using AwesomeAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Loot;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Loot;

[TestFixture]
[TestOf(typeof(LootMapper))]
public class LootMapperTests
{
    private IItemMapper _itemMapper;
    private IMonsterMapper _monsterMapper;
    private LootMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _itemMapper = Substitute.For<IItemMapper>();
        _monsterMapper = Substitute.For<IMonsterMapper>();
        _mapper = new LootMapper(_itemMapper, _monsterMapper);
    }

    [Test]
    public void ToResponse_Loot_MapsFieldsItemsAndMonsters()
    {
        var item = new ItemEntity();
        var itemResponse = new ItemResponse();
        var monster = new MonsterEntity();
        var monsterResponse = new MonsterResponse();
        var loot = new LootEntity
        {
            Id = 3,
            Name = "some-name",
            IsVisibleForPlayer = true,
            GroupId = 4,
            Items = [item],
            Monsters = [monster],
        };

        _itemMapper.ToResponse(item)
            .Returns(itemResponse);
        _monsterMapper.ToResponse(monster)
            .Returns(monsterResponse);

        var actual = _mapper.ToResponse(loot);

        actual.Should().BeEquivalentTo(new LootResponse
        {
            Id = 3,
            Name = "some-name",
            IsVisibleForPlayer = true,
            Items = [itemResponse],
            Monsters = [monsterResponse],
        });
    }
}
