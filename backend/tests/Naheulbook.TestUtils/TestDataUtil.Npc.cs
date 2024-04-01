using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddNpc(Action<NpcEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateNpc(GetLast<GroupEntity>()), customizer);
    }
}