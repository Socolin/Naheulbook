using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class LootRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private LootRepository _lootRepository;
        private Character _character;
        private Group _group;

        [SetUp]
        public void SetUp()
        {
            _lootRepository = new LootRepository(RepositoryDbContext);

            _character = TestDataUtil.AddOrigin().AddUser().AddCharacter().GetLast<Character>();
            _group = TestDataUtil.AddUser().AddGroup(g => g.Characters = new[] {_character}).GetLast<Group>();
        }

        [Test]
        public async Task GetLootsVisibleByCharactersOfGroupAsync_FullyLoadLootAndItems()
        {
            var expectedLoot = TestDataUtil.AddLoot(l => l.IsVisibleForPlayer = true).GetLast<Loot>();
            var expectedLootItemTemplate = TestDataUtil.AddItemTemplateSection().AddItemTemplateCategory().AddItemTemplate().GetLast<ItemTemplate>();
            var expectedLootItem = TestDataUtil.AddItem(expectedLoot).GetLast<Item>();
            var expectedMonster = TestDataUtil.AddMonster(c => c.Loot = expectedLoot).GetLast<Monster>();
            var expectedMonsterItemTemplate = TestDataUtil.AddItemTemplate().GetLast<ItemTemplate>();
            var expectedMonsterItem = TestDataUtil.AddItem(expectedMonster).GetLast<Item>();

            var loots = await _lootRepository.GetLootsVisibleByCharactersOfGroupAsync(_group.Id);

            var loot = loots.First();
            loot.Should().BeEquivalentTo(expectedLoot, config => config.Excluding(l => l.Items).Excluding(l => l.Monsters).Excluding(l => l.Group));
            loot.Items.First().Should().BeEquivalentTo(expectedLootItem, config => config.Excluding(i => i.ItemTemplate).Excluding(i => i.Character).Excluding(i => i.Loot));
            loot.Items.First().ItemTemplate.Should().BeEquivalentTo(expectedLootItemTemplate, config => config.Excluding(i => i.Category).Excluding(i => i.Modifiers).Excluding(i => i.Requirements).Excluding(i => i.Slots).Excluding(i => i.Skills).Excluding(i => i.UnSkills).Excluding(i => i.SkillModifiers));
            loot.Monsters.First().Should().BeEquivalentTo(expectedMonster, config => config.Excluding(m => m.Items).Excluding(m => m.Group).Excluding(m => m.Loot));
            loot.Monsters.First().Items.First().Should().BeEquivalentTo(expectedMonsterItem, config => config.Excluding(i => i.ItemTemplate).Excluding(i => i.Monster));
            loot.Monsters.First().Items.First().ItemTemplate.Should().BeEquivalentTo(expectedMonsterItemTemplate, config => config.Excluding(i => i.Category).Excluding(i => i.Modifiers).Excluding(i => i.Requirements).Excluding(i => i.Slots).Excluding(i => i.Skills).Excluding(i => i.UnSkills).Excluding(i => i.SkillModifiers));
        }

        [Test]
        public async Task GetLootsVisibleByCharactersOfGroupAsync_ShouldNotReturnNonVisibleLoots()
        {
            TestDataUtil.AddLoot(l => l.IsVisibleForPlayer = false).GetLast<Loot>();

            var loots = await _lootRepository.GetLootsVisibleByCharactersOfGroupAsync(_group.Id);

            loots.Should().BeEmpty();
        }

        [Test]
        public async Task GetLootsVisibleByCharactersOfGroupAsync_ShouldNotReturnLootNotAssociateToUserGroup()
        {
            TestDataUtil.AddGroup().AddLoot(l => l.IsVisibleForPlayer = false).GetLast<Loot>();

            var loots = await _lootRepository.GetLootsVisibleByCharactersOfGroupAsync(_group.Id);

            loots.Should().BeEmpty();
        }

        [Test]
        public async Task GetByGroupIdAsync_FullyLoadLootAndItems()
        {
            var expectedLoot = TestDataUtil.AddLoot(l => l.IsVisibleForPlayer = true).GetLast<Loot>();
            var expectedLootItemTemplate = TestDataUtil.AddItemTemplateSection().AddItemTemplateCategory().AddItemTemplate().GetLast<ItemTemplate>();
            var expectedLootItem = TestDataUtil.AddItem(expectedLoot).GetLast<Item>();
            var expectedMonster = TestDataUtil.AddMonster(c => c.Loot = expectedLoot).GetLast<Monster>();
            var expectedMonsterItemTemplate = TestDataUtil.AddItemTemplate().GetLast<ItemTemplate>();
            var expectedMonsterItem = TestDataUtil.AddItem(expectedMonster).GetLast<Item>();

            var loots = await _lootRepository.GetByGroupIdAsync(_group.Id);

            var loot = loots.First();
            loot.Should().BeEquivalentTo(expectedLoot, config => config.Excluding(l => l.Items).Excluding(l => l.Monsters).Excluding(l => l.Group));
            loot.Items.First().Should().BeEquivalentTo(expectedLootItem, config => config.Excluding(i => i.ItemTemplate).Excluding(i => i.Character).Excluding(i => i.Loot));
            loot.Items.First().ItemTemplate.Should().BeEquivalentTo(expectedLootItemTemplate, config => config.Excluding(i => i.Category).Excluding(i => i.Requirements).Excluding(i => i.Modifiers).Excluding(i => i.SkillModifiers).Excluding(i => i.UnSkills).Excluding(i => i.Slots).Excluding(i => i.Skills));
            loot.Monsters.First().Should().BeEquivalentTo(expectedMonster, config => config.Excluding(m => m.Items).Excluding(m => m.Group).Excluding(m => m.Loot));
            loot.Monsters.First().Items.First().Should().BeEquivalentTo(expectedMonsterItem, config => config.Excluding(i => i.ItemTemplate).Excluding(i => i.Monster));
            loot.Monsters.First().Items.First().ItemTemplate.Should().BeEquivalentTo(expectedMonsterItemTemplate, config => config.Excluding(i => i.Category).Excluding(i => i.Requirements).Excluding(i => i.Modifiers).Excluding(i => i.SkillModifiers).Excluding(i => i.UnSkills).Excluding(i => i.Slots).Excluding(i => i.Skills));
        }

        [Test]
        public async Task GetByGroupIdAsync_ShouldNotReturnLootNotAssociateToUserGroup()
        {
            TestDataUtil.AddGroup().AddLoot(l => l.IsVisibleForPlayer = false).GetLast<Loot>();

            var loots = await _lootRepository.GetByGroupIdAsync(_group.Id);

            loots.Should().BeEmpty();
        }
    }
}