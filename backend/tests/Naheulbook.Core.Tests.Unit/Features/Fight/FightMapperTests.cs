using AwesomeAssertions;
using Naheulbook.Core.Features.Fight;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Fight;

[TestFixture]
[TestOf(typeof(FightMapper))]
public class FightMapperTests
{
    private IMonsterMapper _monsterMapper;
    private FightMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _monsterMapper = Substitute.For<IMonsterMapper>();
        _mapper = new FightMapper(_monsterMapper);
    }

    [Test]
    public void ToResponse_Fight_MapsFieldsAndMonsters()
    {
        var monster = new MonsterEntity();
        var monsterResponse = new MonsterResponse();
        var fight = new FightEntity
        {
            Id = 3,
            Name = "some-name",
            GroupId = 4,
            Monsters = [monster],
        };

        _monsterMapper.ToResponse(monster)
            .Returns(monsterResponse);

        var actual = _mapper.ToResponse(fight);

        actual.Should().BeEquivalentTo(new FightResponse
        {
            Id = 3,
            Name = "some-name",
            Monsters = [monsterResponse],
        });
    }
}
