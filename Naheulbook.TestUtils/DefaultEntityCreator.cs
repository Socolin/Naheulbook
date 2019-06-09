using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public class DefaultEntityCreator
    {
        public ItemTemplate CreateItemTemplate(ItemTemplateCategory category, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new ItemTemplate
            {
                Category = category,
                Data = @"{""key"": ""value""}",
                CleanName = $"some-clean-name-{suffix}",
                Name = $"some-item-name-{suffix}",
                Source = "official",
                TechName = $"some-tech-name-{suffix}",
            };
        }

        public ItemTemplateSection CreateItemTemplateSection(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new ItemTemplateSection
            {
                Name = $"some-item-name-{suffix}",
                Note = $"some-note-{suffix}",
                Special = $"some-special-{suffix}"
            };
        }

        public ItemTemplateCategory CreateItemTemplateCategory(ItemTemplateSection section, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new ItemTemplateCategory
            {
                Section = section,
                Note = $"some-note-{suffix}",
                Description = $"some-description-{suffix}",
                TechName = $"some-tech-name-{suffix}",
                Name = $"some-name-{suffix}"
            };
        }

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

        public Slot CreateSlot(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Slot
            {
                Count = 1,
                Name = $"some-name-{suffix}",
                TechName = $"some-tech-name-{suffix}"
            };
        }

        public Location CreateLocation(Location parent = null, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Location
            {
                Name = $"some-name-{suffix}",
                Data = "{}",
                Parent = parent
            };
        }
    }
}