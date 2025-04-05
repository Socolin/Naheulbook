using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddFight(Action<FightEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();

        var fighter = new FightEntity
        {
            Name = RngUtil.GetRandomString("some-fighter-name"),
            GroupId = group.Id,
            Monsters = [],
        };

        return SaveEntity(fighter, customizer);
    }
}