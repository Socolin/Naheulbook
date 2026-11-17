using AwesomeAssertions;
using Naheulbook.Core.Features.Group;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Group;

[TestFixture]
[TestOf(typeof(GroupMapper))]
public class GroupMapperTests
{
    private const string SomeConfigJson = """{"allowPlayersToSeeSkillGmDetails":true,"allowPlayersToAddObject":false}""";

    private GroupMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new GroupMapper();
    }

    [Test]
    public void ToResponse_Group_MapsAllFields()
    {
        var invite = CreateInvite();
        var group = new GroupEntity
        {
            Id = 3,
            Name = "some-name",
            Data = """{"date":{"year":1498}}""",
            Config = SomeConfigJson,
            Characters = [new CharacterEntity {Id = 10}, new CharacterEntity {Id = 11}],
            Invites = [invite],
        };

        var actual = _mapper.ToResponse(group);

        actual.Should().BeEquivalentTo(new GroupResponse
        {
            Id = 3,
            Name = "some-name",
            Data = JObject.Parse("""{"date":{"year":1498}}"""),
            Config = CreateExpectedConfig(),
            CharacterIds = [10, 11],
            Invites = [_mapper.ToGroupInviteResponse(invite)],
        });
    }

    [Test]
    public void ToResponse_GroupWithoutDataAndConfig_ReturnsEmptyDataAndDefaultConfig()
    {
        var group = new GroupEntity
        {
            Name = "some-name",
            Data = null,
            Config = null,
            Characters = [],
            Invites = [],
        };

        var actual = _mapper.ToResponse(group);

        actual.Data.Should().BeEquivalentTo(new JObject());
        actual.Config.Should().BeEquivalentTo(new GroupConfig());
    }

    [Test]
    public void ToSummaryResponse_Group_MapsIdNameAndCharacterCount()
    {
        var group = new GroupEntity
        {
            Id = 3,
            Name = "some-name",
            Characters = [new CharacterEntity(), new CharacterEntity()],
        };

        var actual = _mapper.ToSummaryResponse(group);

        actual.Should().BeEquivalentTo(new GroupSummaryResponse
        {
            Id = 3,
            Name = "some-name",
            CharacterCount = 2,
        });
    }

    [Test]
    public void ToCharacterGroupResponse_Group_MapsIdNameAndConfig()
    {
        var group = new GroupEntity
        {
            Id = 3,
            Name = "some-name",
            Config = SomeConfigJson,
        };

        var actual = _mapper.ToCharacterGroupResponse(group);

        actual.Should().BeEquivalentTo(new CharacterGroupResponse
        {
            Id = 3,
            Name = "some-name",
            Config = CreateExpectedConfig(),
        });
    }

    [Test]
    public void ToResponse_GroupHistoryEntry_MapsAllFields()
    {
        var historyEntry = new GroupHistoryEntryEntity
        {
            Id = 3,
            Action = "some-action",
            Data = """{"key":"value"}""",
            Date = new DateTime(2024, 3, 4, 5, 6, 7, DateTimeKind.Utc),
            Gm = true,
            Info = "some-info",
            GroupId = 4,
        };

        var actual = _mapper.ToResponse(historyEntry);

        actual.Should().BeEquivalentTo(new GroupHistoryEntryResponse
        {
            Id = 3,
            Action = "some-action",
            Data = new JObject {["key"] = "value"},
            Date = "2024-03-04T05:06:07",
            Gm = true,
            Info = "some-info",
        });
    }

    [Test]
    public void ToResponse_GroupHistoryEntryWithoutData_ReturnsEmptyData()
    {
        var historyEntry = new GroupHistoryEntryEntity {Action = "some-action", Data = null};

        var actual = _mapper.ToResponse(historyEntry);

        actual.Data.Should().BeEquivalentTo(new JObject());
    }

    [Test]
    public void ToDeleteInviteResponse_GroupInvite_MapsIdsAndFromGroup()
    {
        var invite = new GroupInviteEntity {GroupId = 3, CharacterId = 4, FromGroup = true};

        var actual = _mapper.ToDeleteInviteResponse(invite);

        actual.Should().BeEquivalentTo(new DeleteInviteResponse
        {
            GroupId = 3,
            CharacterId = 4,
            FromGroup = true,
        });
    }

    [Test]
    public void ToInviteResponse_GroupInvite_MapsCharacterInfoAndGroup()
    {
        var invite = CreateInvite();

        var actual = _mapper.ToInviteResponse(invite);

        actual.Should().BeEquivalentTo(new GroupInviteResponse
        {
            Id = 10,
            Name = "some-character-name",
            FromGroup = true,
            Group = _mapper.ToCharacterGroupResponse(invite.Group),
            OriginName = "some-origin-name",
            JobNames = ["some-job-1", "some-job-2"],
        });
    }

    [Test]
    public void ToGroupInviteResponse_GroupInvite_MapsCharacterInfo()
    {
        var invite = CreateInvite();

        var actual = _mapper.ToGroupInviteResponse(invite);

        actual.Should().BeEquivalentTo(new GroupGroupInviteResponse
        {
            Id = 10,
            Name = "some-character-name",
            FromGroup = true,
            OriginName = "some-origin-name",
            JobNames = ["some-job-1", "some-job-2"],
        });
    }

    [Test]
    public void ToCharacterInviteResponse_GroupInvite_MapsGroupInfo()
    {
        var invite = CreateInvite();

        var actual = _mapper.ToCharacterInviteResponse(invite);

        actual.Should().BeEquivalentTo(new CharacterGroupInviteResponse
        {
            GroupId = 3,
            GroupName = "some-group-name",
            Config = CreateExpectedConfig(),
        });
    }

    private static GroupInviteEntity CreateInvite()
    {
        return new GroupInviteEntity
        {
            FromGroup = true,
            GroupId = 3,
            Group = new GroupEntity {Id = 3, Name = "some-group-name", Config = SomeConfigJson},
            CharacterId = 10,
            Character = new CharacterEntity
            {
                Id = 10,
                Name = "some-character-name",
                Origin = new OriginEntity {Name = "some-origin-name"},
                Jobs =
                [
                    new CharacterJobEntity {Job = new JobEntity {Name = "some-job-1"}},
                    new CharacterJobEntity {Job = new JobEntity {Name = "some-job-2"}},
                ],
            },
        };
    }

    private static GroupConfig CreateExpectedConfig()
    {
        return new GroupConfig
        {
            AllowPlayersToSeeSkillGmDetails = true,
            AllowPlayersToAddObject = false,
        };
    }
}
