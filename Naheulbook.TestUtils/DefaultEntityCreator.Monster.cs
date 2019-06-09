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
    }
}