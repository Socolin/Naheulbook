using AwesomeAssertions;
using Naheulbook.Core.Features.Npc;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Npc;

[TestFixture]
[TestOf(typeof(NpcMapper))]
public class NpcMapperTests
{
    private NpcMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new NpcMapper();
    }

    [Test]
    public void ToResponse_Npc_MapsFieldsAndDeserializeData()
    {
        var npc = new NpcEntity
        {
            Id = 3,
            Name = "some-name",
            Data = """{"location":"some-location","sex":"some-sex"}""",
            GroupId = 8,
        };

        var actual = _mapper.ToResponse(npc);

        actual.Should().BeEquivalentTo(new NpcResponse
        {
            Id = 3,
            Name = "some-name",
            Data = new NpcData {Location = "some-location", Sex = "some-sex"},
        });
    }
}
