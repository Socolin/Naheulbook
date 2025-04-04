using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddNpc(Action<NpcEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();
        var npc = new NpcEntity
        {
            Data = "{}",
            GroupId = group.Id,
            Name = RngUtil.GetRandomString("some-npc-name"),
        };

        return SaveEntity(npc, customizer);
    }
}