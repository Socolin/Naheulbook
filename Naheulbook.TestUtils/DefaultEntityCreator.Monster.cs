using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public MonsterCategory CreateMonsterCategory(MonsterType monsterType, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new MonsterCategory
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
                Categories = new List<MonsterCategory>()
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

        public MonsterTemplate CreateMonsterTemplate(MonsterCategory category, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new MonsterTemplate
            {
                Name = $"some-monster-template-name-{suffix}",
                Category = category,
                Data = @"{""key"":""value""}"
            };
        }
    }
}