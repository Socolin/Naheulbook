using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public ItemTemplate CreateItemTemplate(ItemTemplateCategory category, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new ItemTemplate
            {
                Category = category,
                Data = @"{""key"": ""value""}",
                CleanName = $"some-item-name-{suffix}",
                Name = $"some-item-name-{suffix}",
                Source = "official",
                TechName = $"some-tech-name-{suffix}"
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

        public Item CreateItem(ItemTemplate itemTemplate, Character character)
        {
            return new Item
            {
                Data = @"{""key"":""value""}",
                Modifiers = JsonConvert.SerializeObject(new List<ActiveStatsModifier>
                {
                    new ActiveStatsModifier
                    {
                        Active = true,
                        Description = "some-description"
                    }
                }, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                ItemTemplateId = itemTemplate.Id,
                CharacterId = character.Id,
                Character = character
            };
        }

        public Item CreateItem(ItemTemplate itemTemplate, Monster monster)
        {
            return new Item
            {
                Data = @"{""key"":""value""}",
                Modifiers = @"[{""key"":""value""}]",
                ItemTemplateId = itemTemplate.Id,
                MonsterId = monster.Id,
                Monster = monster
            };
        }

        public Item CreateItem(ItemTemplate itemTemplate, Loot loot)
        {
            return new Item
            {
                Data = @"{""key"":""value""}",
                Modifiers = @"[{""key"":""value""}]",
                ItemTemplateId = itemTemplate.Id,
                LootId = loot.Id,
                Loot = loot
            };
        }
    }
}