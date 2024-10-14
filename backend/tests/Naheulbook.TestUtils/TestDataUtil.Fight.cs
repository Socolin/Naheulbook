using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddFight(Action<FightEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateFight(GetLast<GroupEntity>()), customizer);
    }
}