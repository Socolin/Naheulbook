using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class DefaultEntityCreator
{
    public FightEntity CreateFight(GroupEntity group, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new FightEntity
        {
            Name = $"some-name-{suffix}",
            Group = group,
            GroupId = group.Id,
            Monsters = [],
        };
    }
}