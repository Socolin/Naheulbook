using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddMonsterType(Action<MonsterTypeEntity> customizer = null)
    {
        return AddMonsterType(out _, customizer);
    }

    public TestDataUtil AddMonsterType(out MonsterTypeEntity monsterType, Action<MonsterTypeEntity> customizer = null)
    {
        monsterType = new MonsterTypeEntity
        {
            Name = RngUtil.GetRandomString("some-monster-type-name"),
            SubCategories = new List<MonsterSubCategoryEntity>(),
        };

        return SaveEntity(monsterType, customizer);
    }

    public TestDataUtil AddMonsterSubCategory(Action<MonsterSubCategoryEntity> customizer = null)
    {
        var monsterSubCategory = new MonsterSubCategoryEntity
        {
            MonsterTemplates = new List<MonsterTemplateEntity>(),
            Name = RngUtil.GetRandomString("some-name"),
            TypeId = GetLast<MonsterTypeEntity>().Id,
        };

        return SaveEntity(monsterSubCategory, customizer);
    }

    public TestDataUtil AddMonsterTrait(Action<MonsterTraitEntity> customizer = null)
    {
        return AddMonsterTrait(out _, customizer);
    }

    public TestDataUtil AddMonsterTrait(out MonsterTraitEntity monsterTrait, Action<MonsterTraitEntity> customizer = null)
    {
        monsterTrait = new MonsterTraitEntity
        {
            Name = RngUtil.GetRandomString("some-monster-trait-name"),
            Description = RngUtil.GetRandomString("some-monster-trait-description"),
            Levels = @"[""level1"", ""level2""]",
        };

        return SaveEntity(monsterTrait, customizer);
    }

    public TestDataUtil AddMonsterTemplate(Action<MonsterTemplateEntity> customizer = null)
    {
        return AddMonsterTemplate(out _, customizer);
    }

    public TestDataUtil AddMonsterTemplate(out MonsterTemplateEntity monsterTemplate, Action<MonsterTemplateEntity> customizer = null)
    {
        monsterTemplate = new MonsterTemplateEntity
        {
            Name = RngUtil.GetRandomString("some-monster-template-name"),
            SubCategoryId = GetLast<MonsterSubCategoryEntity>().Id,
            Data = """{"key":"value"}""",
        };
        return SaveEntity(monsterTemplate, customizer);
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