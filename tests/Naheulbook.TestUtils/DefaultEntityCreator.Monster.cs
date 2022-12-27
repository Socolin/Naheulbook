using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils;

public partial class DefaultEntityCreator
{
    public MonsterSubCategoryEntity CreateMonsterSubCategory(MonsterTypeEntity monsterType, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new MonsterSubCategoryEntity
        {
            MonsterTemplates = new List<MonsterTemplateEntity>(),
            Name = $"some-name-{suffix}",
            Type = monsterType,
            TypeId = monsterType.Id,
        };
    }

    public MonsterTypeEntity CreateMonsterType(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new MonsterTypeEntity()
        {
            Name = $"some-monster-type-name-{suffix}",
            SubCategories = new List<MonsterSubCategoryEntity>(),
        };
    }

    public MonsterTraitEntity CreateMonsterTrait(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new MonsterTraitEntity
        {
            Name = $"some-monster-trait-name-{suffix}",
            Description = $"some-monster-trait-description-{suffix}",
            Levels = @"[""level1"", ""level2""]",
        };
    }

    public MonsterTemplateEntity CreateMonsterTemplate(MonsterSubCategoryEntity subCategory, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new MonsterTemplateEntity
        {
            Name = $"some-monster-template-name-{suffix}",
            SubCategory = subCategory,
            Data = @"{""key"":""value""}",
        };
    }

    public MonsterEntity CreateMonster(GroupEntity group, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new MonsterEntity
        {
            Name = $"some-monster-name-{suffix}",
            Group = group,
            GroupId = group.Id,
            Data = @"{""key"": ""value""}",
        };
    }
}