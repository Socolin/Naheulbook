using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class CharacterRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private static readonly DateTimeOffset NowUtc = new(2025, 03, 28, 10, 12, 8, TimeSpan.Zero);
    private CharacterRepository _characterRepository;

    [SetUp]
    public void SetUp()
    {
        _characterRepository = new CharacterRepository(RepositoryDbContext);
    }

    #region GetWithAllDataAsync

    [Test]
    public async Task GetWithAllDataAsync_ShouldLoadExpectedCharacter_WithAllExpectedData()
    {
        TestDataUtil
            .AddUser()
            .AddGroup(out var group)
            .AddCharacterWithAllData(out var character)
            .AddGroupInvite(out var groupInvite, true);

        var actualCharacter = await _characterRepository.GetWithAllDataAsync(character.Id);

        AssertEntityIsLoaded(actualCharacter, character);
        AssertEntitiesAreLoaded(actualCharacter.Jobs, TestDataUtil.GetAll<CharacterJobEntity>());
        AssertEntitiesAreLoaded(actualCharacter.Specialities.Select(x => x.Speciality), TestDataUtil.GetAll<SpecialityEntity>());
        AssertEntityIsLoaded(actualCharacter.Group, group);
        AssertEntitiesAreLoaded(actualCharacter.Invites, [groupInvite]);
        AssertEntitiesAreLoaded(actualCharacter.Modifiers, TestDataUtil.GetAll<CharacterModifierEntity>());
    }

    #endregion

    #region GetWithGroupAsync

    #endregion

    #region GetForSummaryByOwnerIdAsync

    [Test]
    public async Task GetForSummaryByOwnerIdAsync_LoadCharacterOwnedByUser_IncludingJobs()
    {
        TestDataUtil
            .AddUser(out var user)
            .AddJob(out var job)
            .AddOrigin()
            .AddCharacter(out var character)
            .AddCharacterJob(out var characterJob)
            ;

        var actualEntities = await _characterRepository.GetForSummaryByOwnerIdAsync(user.Id);

        AssertEntitiesAreLoaded(actualEntities, [character]);
        var actualCharacter = actualEntities.Single();
        AssertEntitiesAreLoaded(actualCharacter.Jobs, [characterJob]);
        AssertEntityIsLoaded(actualCharacter.Jobs.Single().Job, job);
    }

    [Test]
    public async Task GetForSummaryByOwnerIdAsync_ShouldNotReturnsCharacterThatDoesNotBelongToTheUser()
    {
        TestDataUtil
            .AddJob()
            .AddOrigin()
            .AddUser(out var notOwnerUser)
            .AddUser(out var owner)
            .AddCharacter(out var character);

        var actualCharacters = await _characterRepository.GetForSummaryByOwnerIdAsync(notOwnerUser.Id);
        var existingCharacters = await _characterRepository.GetForSummaryByOwnerIdAsync(owner.Id);

        actualCharacters.Should().BeEmpty();
        AssertEntitiesAreLoaded(existingCharacters, [character]);
    }

    #endregion

    #region GetHistoryByCharacterIdAsync

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldSortResultByDateDescending()
    {
        TestDataUtil
            .AddUser()
            .AddOrigin()
            .AddCharacter(out var character)
            .AddCharacterHistoryEntry(out var characterHistoryEntry1, h => h.Date = NowUtc.AddMinutes(100).UtcDateTime)
            .AddCharacterHistoryEntry(out var characterHistoryEntry2, h => h.Date = NowUtc.AddMinutes(300).UtcDateTime);

        var actualEntities = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, null, 0, false);

        AssertEntitiesAreLoadedWithSameOrder(actualEntities.Cast<CharacterHistoryEntryEntity>(), [characterHistoryEntry2, characterHistoryEntry1]);
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldSortGroupAndCharacterResultByDateDescending()
    {
        TestDataUtil
            .AddUser()
            .AddGroup()
            .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
            .AddCharacterHistoryEntry(out var characterHistoryEntry1, h => h.Date = NowUtc.AddMinutes(100).UtcDateTime)
            .AddGroupHistoryEntry(out var groupHistoryEntry1, h => h.Date = NowUtc.AddMinutes(200).UtcDateTime)
            .AddCharacterHistoryEntry(out var characterHistoryEntry2, h => h.Date = NowUtc.AddMinutes(300).UtcDateTime);

        var character = TestDataUtil.Get<CharacterEntity>(0);
        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

        historyEntries.Select(h => h.Action).Should().BeEquivalentTo([
                characterHistoryEntry2.Action,
                groupHistoryEntry1.Action,
                characterHistoryEntry1.Action,
            ],
            opt => opt.WithStrictOrdering()
        );
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldNotLoadHistoryEntryThatDoesNotBelongToSpecifiedCharacter()
    {
        TestDataUtil
            .AddCharacterWithRequiredDependencies()
            .AddCharacterWithRequiredDependencies()
            .AddCharacterHistoryEntry();

        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<CharacterEntity>(0).Id, null, 0, false);

        historyEntries.Should().BeEmpty();
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldNotLoadGmEntryWhenGmFlagIsSet()
    {
        TestDataUtil
            .AddUser()
            .AddGroup()
            .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
            .AddCharacterHistoryEntry(h =>
                {
                    h.Date = NowUtc.AddMinutes(100).UtcDateTime;
                    h.Gm = true;
                }
            )
            .AddGroupHistoryEntry(h =>
                {
                    h.Date = NowUtc.AddMinutes(200).UtcDateTime;
                    h.Gm = true;
                }
            );

        var character = TestDataUtil.Get<CharacterEntity>();
        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

        historyEntries.Should().BeEmpty();
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldLoadGmEntryTooWhenIsGmIsTrue()
    {
        TestDataUtil
            .AddUser()
            .AddGroup()
            .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
            .AddCharacterHistoryEntry(h =>
                {
                    h.Date = NowUtc.AddMinutes(100).UtcDateTime;
                    h.Gm = true;
                }
            )
            .AddGroupHistoryEntry(h =>
                {
                    h.Date = NowUtc.AddMinutes(200).UtcDateTime;
                    h.Gm = true;
                }
            )
            .AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(400).UtcDateTime)
            .AddGroupHistoryEntry(h => h.Date = NowUtc.AddMinutes(300).UtcDateTime);

        var character = TestDataUtil.Get<CharacterEntity>();
        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, true);

        historyEntries.Should().HaveCount(4);
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldReturnsGroupHistory_MoreRecentThanTheLatestChararacterHistoryWhenRequestingPage0()
    {
        TestDataUtil
            .AddUser()
            .AddGroup()
            .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
            .AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(100).UtcDateTime)
            .AddGroupHistoryEntry(h => h.Date = NowUtc.AddMinutes(200).UtcDateTime);

        var character = TestDataUtil.Get<CharacterEntity>();
        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

        historyEntries.Select(c => c.Action).Should().BeEquivalentTo(
            TestDataUtil.GetLast<GroupHistoryEntryEntity>().Action,
            TestDataUtil.GetLast<CharacterHistoryEntryEntity>().Action
        );
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldReturnsGroupHistoryThatHappenedBetweenCharacterActions()
    {
        TestDataUtil
            .AddUser()
            .AddGroup()
            .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
            .AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(100).UtcDateTime)
            .AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(400).UtcDateTime)
            .AddGroupHistoryEntry(h => h.Date = NowUtc.AddMinutes(200).UtcDateTime)
            .AddGroupHistoryEntry(h => h.Date = NowUtc.AddMinutes(300).UtcDateTime)
            .AddGroupHistoryEntry(h => h.Date = NowUtc.AddMinutes(50).UtcDateTime)
            ;

        var character = TestDataUtil.Get<CharacterEntity>();
        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

        historyEntries.Select(c => c.Action).Should().Contain(
            TestDataUtil.Get<GroupHistoryEntryEntity>(1).Action,
            TestDataUtil.Get<GroupHistoryEntryEntity>(0).Action
        );
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldNotReturnsEntryOfOtherGroups()
    {
        TestDataUtil
            .AddUser()
            .AddGroup()
            .AddGroupHistoryEntry(h => h.Date = NowUtc.AddMinutes(200).UtcDateTime)
            .AddGroup()
            .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
            .AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(100).UtcDateTime)
            .AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(400).UtcDateTime);

        var character = TestDataUtil.Get<CharacterEntity>();

        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

        historyEntries.OfType<GroupHistoryEntryEntity>().Should().BeEmpty();
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldReturnOnePageAtTheTime()
    {
        TestDataUtil
            .AddCharacterWithRequiredDependencies();
        for (var i = 0; i < 41; i++)
        {
            var i1 = i;
            TestDataUtil.AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(i1 * 100).UtcDateTime);
        }

        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<CharacterEntity>(0).Id, null, 0, false);

        historyEntries.Should().HaveCount(40);
    }

    [Test]
    public async Task GetHistoryByCharacterIdAsync_ShouldReturnRequestedPage()
    {
        TestDataUtil
            .AddCharacterWithRequiredDependencies();

        for (var i = 0; i < 41; i++)
        {
            var i1 = i;
            TestDataUtil.AddCharacterHistoryEntry(h => h.Date = NowUtc.AddMinutes(i1 * 100000).UtcDateTime);
        }

        var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<CharacterEntity>(0).Id, null, 1, false);

        historyEntries.Should().HaveCount(1);
        historyEntries.First().Action.Should().Be(TestDataUtil.Get<CharacterHistoryEntryEntity>(0).Action);
    }

    #endregion

    #region SearchCharacterWithNoGroupByNameWithOriginWithOwner

    #endregion

    #region GetWithGroupWithJobsWithOriginAsync

    [Test]
    public async Task GetWithGroupWithJobsWithOriginAsync_ShouldIncludeRelatedEntities()
    {
        TestDataUtil
            .AddJob(out var job1)
            .AddJob(out var job2)
            .AddOrigin(out var origin)
            .AddUser()
            .AddGroup(out var group)
            .AddCharacter(out var character)
            .AddCharacterJob(out var characterJob1, job1)
            .AddCharacterJob(out var characterJob2, job2);

        var actualCharacter = await _characterRepository.GetWithGroupWithJobsWithOriginAsync(character.Id);

        AssertEntityIsLoaded(actualCharacter, character);
        AssertEntitiesAreLoaded(actualCharacter.Jobs, [characterJob1, characterJob2]);
        AssertEntityIsLoaded(actualCharacter.Jobs.Single(x => x.JobId == job1.Id).Job, job1);
        AssertEntityIsLoaded(actualCharacter.Jobs.Single(x => x.JobId == job2.Id).Job, job2);
        AssertEntityIsLoaded(actualCharacter.Origin, origin);
        AssertEntityIsLoaded(actualCharacter.Group, group);
    }

    #endregion

    #region GetWithItemsWithModifiersByGroupAndByIdAsync

    #endregion
}