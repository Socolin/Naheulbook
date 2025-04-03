using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddMonsterType(Action<MonsterTypeEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateMonsterType(), customizer);
    }

    public TestDataUtil AddMonsterSubCategory(Action<MonsterSubCategoryEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateMonsterSubCategory(GetLast<MonsterTypeEntity>()), customizer);
    }

    public TestDataUtil AddMonsterTrait(Action<MonsterTraitEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateMonsterTrait(), customizer);
    }

    public TestDataUtil AddMonsterTemplate(Action<MonsterTemplateEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateMonsterTemplate(GetLast<MonsterSubCategoryEntity>()), customizer);
    }

    public TestDataUtil AddMonster(Action<MonsterEntity> customizer = null)
    {
        return AddMonster(out _, customizer);
    }

    public TestDataUtil AddMonster(out MonsterEntity monster, Action<MonsterEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();

        monster = new MonsterEntity
        {
            Name = RngUtil.GetRandomString("some-monster-name"),
            GroupId = group.Id,
            Data = """{"key": "value"}""",
        };

        return SaveEntity(monster, customizer);
    }
}