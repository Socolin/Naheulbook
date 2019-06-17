using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class ItemRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private ItemRepository _itemRepository;

        [SetUp]
        public void SetUp()
        {
            _itemRepository = new ItemRepository(RepositoryDbContext);
        }

        [Test]
        public async Task GetWithAllDataAsync_ShouldLoadAllRelatedData()
        {
            var user = TestDataUtil.AddUser().GetLast<User>();

            TestDataUtil.AddItemTemplateWithAllData();

            var character = TestDataUtil.AddOrigin().AddCharacter(user.Id).GetLast<Character>();

            TestDataUtil.AddItem(character);

            var item = await _itemRepository.GetWithAllDataAsync(TestDataUtil.Get<ItemTemplate>().Id);

            item.Should().BeEquivalentTo(TestDataUtil.GetLast<Item>(), config => config.Excluding(x => x.Character).Excluding(x => x.Loot).Excluding(x => x.Monster).Excluding(x => x.ItemTemplate));
            var itemTemplate = item.ItemTemplate;
            itemTemplate.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>(), config => config.Excluding(x => x.Category).Excluding(x => x.Requirements).Excluding(x => x.Modifiers).Excluding(x => x.Skills).Excluding(x => x.UnSkills).Excluding(x => x.SkillModifiers).Excluding(x => x.Slots));
            itemTemplate.Modifiers.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Modifiers, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.RequireJob).Excluding(x => x.RequireOrigin).Excluding(x => x.Stat));
            itemTemplate.Requirements.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Requirements, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Stat));
            itemTemplate.Skills.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Skills, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Skill));
            itemTemplate.UnSkills.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().UnSkills, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Skill));
            itemTemplate.SkillModifiers.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().SkillModifiers, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Skill));
            itemTemplate.Slots.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Slots, config => config.Excluding(x => x.ItemTemplate).Excluding(x => x.Slot));
            itemTemplate.Slots.First().Slot.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplate>().Slots.First().Slot);
        }
    }
}