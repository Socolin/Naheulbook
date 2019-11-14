using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public MonsterSubCategory CreateMonsterSubCategory(MonsterType monsterType, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new MonsterSubCategory
            {
                MonsterTemplates = new List<MonsterTemplate>(),
                Name = $"some-name-{suffix}",
                Type = monsterType,
                TypeId = monsterType.Id
            };
        }

        public MonsterType CreateMonsterType(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new MonsterType()
            {
                Name = $"some-monster-type-name-{suffix}",
                SubCategories = new List<MonsterSubCategory>()
            };
        }

        public MonsterTrait CreateMonsterTrait(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new MonsterTrait
            {
                Name = $"some-monster-trait-name-{suffix}",
                Description = $"some-monster-trait-description-{suffix}",
                Levels = @"[""level1"", ""level2""]"
            };
        }

        public MonsterTemplate CreateMonsterTemplate(MonsterSubCategory subCategory, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new MonsterTemplate
            {
                Name = $"some-monster-template-name-{suffix}",
                SubCategory = subCategory,
                Data = @"{""key"":""value""}"
            };
        }

        public Monster CreateMonster(Group group, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Monster
            {
                Name = $"some-monster-name-{suffix}",
                Group = group,
                GroupId = group.Id,
                Data = @"{""key"": ""value""}"
           };
        }
    }
}