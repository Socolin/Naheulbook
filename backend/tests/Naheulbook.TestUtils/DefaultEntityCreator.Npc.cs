using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class DefaultEntityCreator
{
    public NpcEntity CreateNpc(GroupEntity group, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new NpcEntity()
        {
            Data = "{}",
            GroupId = group.Id,
            Name = $"some-npc-name-{suffix}",
        };
    }
}