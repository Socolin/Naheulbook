using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.TestUtils.Extensions;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class CharacterRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
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
            TestDataUtil.AddUser();
            var user = TestDataUtil.GetLast<UserEntity>();
            TestDataUtil.AddCharacterWithAllData(user.Id);
            var expectedCharacter = TestDataUtil.GetLast<CharacterEntity>();
            TestDataUtil.AddGroupInvite(expectedCharacter, TestDataUtil.GetLast<GroupEntity>(), true);

            var character = await _characterRepository.GetWithAllDataAsync(expectedCharacter.Id);

            character.Should().BeEquivalentTo(expectedCharacter, config => config.ExcludingChildren());
            character!.Jobs.Select(x => x.JobId).Should().BeEquivalentTo(TestDataUtil.GetAll<JobEntity>().Select(x => x.Id));
            character!.Specialities.Select(x => x.Speciality).Should().BeEquivalentTo(TestDataUtil.GetAll<SpecialityEntity>(), config => config.ExcludingChildren());
            character!.Group.Should().BeEquivalentTo(TestDataUtil.GetLast<GroupEntity>(), config => config.ExcludingChildren());
            character!.Modifiers.Should().BeEquivalentTo(expectedCharacter.Modifiers, config => config.ExcludingChildren());
            character!.Invites.Should().BeEquivalentTo(TestDataUtil.GetAll<GroupInviteEntity>(), config => config.ExcludingChildren());
            character!.Invites.First().Group.Should().BeEquivalentTo(TestDataUtil.Get<GroupEntity>(), config => config.ExcludingChildren());
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

            AssertEntitiesAreLoaded(actualEntities, new[] {character});
            var actualCharacter = actualEntities.Single();
            AssertEntitiesAreLoaded(actualCharacter.Jobs, new[] {characterJob});
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
            AssertEntitiesAreLoaded(existingCharacters, new[] {character});
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
                .AddCharacterHistoryEntry(out var characterHistoryEntry1, h => h.Date = new DateTime(100))
                .AddCharacterHistoryEntry(out var characterHistoryEntry2, h => h.Date = new DateTime(300));

            var actualEntities = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, null, 0, false);

            AssertEntitiesAreLoadedWithSameOrder(actualEntities.Cast<CharacterHistoryEntryEntity>(), new [] {characterHistoryEntry2, characterHistoryEntry1});
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldSortGroupAndCharacterResultByDateDescending()
        {
            TestDataUtil
                .AddUser()
                .AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
                .AddCharacterHistoryEntry(out var characterHistoryEntry1, h => h.Date = new DateTime(100))
                .AddGroupHistoryEntry(out var groupHistoryEntry1, h => h.Date = new DateTime(200))
                .AddCharacterHistoryEntry(out var characterHistoryEntry2, h => h.Date = new DateTime(300));

            var character = TestDataUtil.Get<CharacterEntity>(0);
            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

            historyEntries.Select(h => h.Action).Should().BeEquivalentTo(new[]
                {
                    characterHistoryEntry2.Action,
                    groupHistoryEntry1.Action,
                    characterHistoryEntry1.Action,
                }, opt => opt.WithStrictOrdering()
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
                    h.Date = new DateTime(100);
                    h.Gm = true;
                })
                .AddGroupHistoryEntry(h =>
                {
                    h.Date = new DateTime(200);
                    h.Gm = true;
                });

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
                    h.Date = new DateTime(100);
                    h.Gm = true;
                })
                .AddGroupHistoryEntry(h =>
                {
                    h.Date = new DateTime(200);
                    h.Gm = true;
                })
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(400))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(300));

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
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(200));

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
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(400))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(200))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(300))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(50))
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
                .AddGroupHistoryEntry(h => h.Date = new DateTime(200))
                .AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<GroupEntity>())
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(400));

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
                TestDataUtil.AddCharacterHistoryEntry(h => h.Date = new DateTime(i1 * 100));
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
                TestDataUtil.AddCharacterHistoryEntry(h => h.Date = new DateTime(i1 * 100));
            }

            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<CharacterEntity>(0).Id, null, 1, false);

            historyEntries.Should().HaveCount(1);
            historyEntries.First().Action.Should().Be(TestDataUtil.Get<CharacterHistoryEntryEntity>(0).Action);
        }

        #endregion

        #region SearchCharacterWithNoGroupByNameWithOriginWithOwner

        #endregion

        #region GetWithOriginWithJobsAsync

        #endregion

        #region GetWithItemsWithModifiersByGroupAndByIdAsync

        #endregion
    }
}