using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class LootRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private LootRepository _lootRepository;
    [SetUp]
    public void SetUp()
    {
        _lootRepository = new LootRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetLootsVisibleByCharactersOfGroupAsync_FullyLoadLootAndItems()
    {
        TestDataUtil
            .AddUser()
            .AddGroup(out var group)
            .AddLoot(out var loot)
            .AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddItemTemplate(out var lootItemTemplate)
            .AddItemToLoot(out var lootItem)
            .AddMonster(out var monster, m => m.LootId = loot.Id)
            .AddItemTemplate(out var monsterItemTemplate)
            .AddItemToMonster(out var monsterItem)
            ;

        var loots = await _lootRepository.GetLootsVisibleByCharactersOfGroupAsync(group.Id);

        var actualLoot = loots.First();
        AssertEntityIsLoaded(actualLoot, loot);
        AssertEntitiesAreLoaded(actualLoot.Items, [lootItem]);
        AssertEntityIsLoaded(actualLoot.Items.Single().ItemTemplate, lootItemTemplate);
        AssertEntitiesAreLoaded(actualLoot.Monsters, [monster]);
        AssertEntitiesAreLoaded(actualLoot.Monsters.Single().Items, [monsterItem]);
        AssertEntityIsLoaded(actualLoot.Monsters.Single().Items.Single().ItemTemplate, monsterItemTemplate);
    }

    [Test]
    public async Task GetLootsVisibleByCharactersOfGroupAsync_ShouldNotReturnNonVisibleLoots()
    {
        TestDataUtil.AddLoot(out var loot, l => l.IsVisibleForPlayer = false);

        var loots = await _lootRepository.GetLootsVisibleByCharactersOfGroupAsync(loot.Id);

        loots.Should().BeEmpty();
    }

    [Test]
    public async Task GetLootsVisibleByCharactersOfGroupAsync_ShouldNotReturnLootNotAssociateToUserGroup()
    {
        TestDataUtil
            .AddGroup(out var otherGroup)
            .AddGroup(out var group)
            .AddLoot(out var loot, l => l.IsVisibleForPlayer = false);

        var actualLoots = await _lootRepository.GetLootsVisibleByCharactersOfGroupAsync(otherGroup.Id);
        var existingLoots = await _lootRepository.GetLootsVisibleByCharactersOfGroupAsync(group.Id);

        actualLoots.Should().BeEmpty();
        AssertEntitiesAreLoaded(existingLoots, [loot]);
    }

    [Test]
    public async Task GetByGroupIdAsync_FullyLoadLootAndItems()
    {
        TestDataUtil
            .AddUser()
            .AddGroup(out var group)
            .AddLoot(out var loot)
            .AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddItemTemplate(out var lootItemTemplate)
            .AddItemToLoot(out var lootItem)
            .AddMonster(out var monster, m => m.LootId = loot.Id)
            .AddItemTemplate(out var monsterItemTemplate)
            .AddItemToMonster(out var monsterItem)
            ;

        var actualLoots = await _lootRepository.GetByGroupIdAsync(group.Id);

        var actualLoot = actualLoots.First();
        AssertEntityIsLoaded(actualLoot, loot);
        AssertEntitiesAreLoaded(actualLoot.Items, [lootItem]);
        AssertEntityIsLoaded(actualLoot.Items.Single().ItemTemplate, lootItemTemplate);
        AssertEntitiesAreLoaded(actualLoot.Monsters, [monster]);
        AssertEntitiesAreLoaded(actualLoot.Monsters.Single().Items, [monsterItem]);
        AssertEntityIsLoaded(actualLoot.Monsters.Single().Items.Single().ItemTemplate, monsterItemTemplate);
    }

    [Test]
    public async Task GetByGroupIdAsync_ShouldNotReturnLootNotAssociateToUserGroup()
    {
        TestDataUtil
            .AddGroup(out var otherGroup)
            .AddGroup(out var group)
            .AddLoot(out var loot, l => l.IsVisibleForPlayer = false).GetLast<LootEntity>();

        var actualLoots = await _lootRepository.GetByGroupIdAsync(otherGroup.Id);
        var existingLoots = await _lootRepository.GetByGroupIdAsync(group.Id);

        actualLoots.Should().BeEmpty();
        AssertEntitiesAreLoaded(existingLoots, [loot]);
    }
}