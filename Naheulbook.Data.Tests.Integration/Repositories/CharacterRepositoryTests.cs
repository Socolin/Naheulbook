using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
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

        [Test]
        public async Task GetWithAllDataAsync_ShouldLoadExpectedCharacter_WithAllExpectedData()
        {
            TestDataUtil.AddUser();
            var user = TestDataUtil.GetLast<User>();
            TestDataUtil.AddCharacterWithAllData(user.Id);
            var expectedCharacter = TestDataUtil.GetLast<Character>();

            var character = await _characterRepository.GetWithAllDataAsync(expectedCharacter.Id);

            character.Should().BeEquivalentTo(expectedCharacter, config => config.Excluding(x => x.Owner).Excluding(x => x.Jobs).Excluding(x => x.Origin).Excluding(x => x.Modifiers).Excluding(x => x.Skills).Excluding(x => x.Group).Excluding(x => x.Specialities));
            character.Jobs.Select(x => x.JobId).Should().BeEquivalentTo(TestDataUtil.GetAll<Job>().Select(x => x.Id));
            character.Specialities.Select(x => x.Speciality).Should().BeEquivalentTo(TestDataUtil.GetAll<Speciality>(), config => config.Excluding(x => x.Job));
            character.Group.Should().BeEquivalentTo(TestDataUtil.GetLast<Group>(), config => config.Excluding(x => x.Characters).Excluding(x => x.Master).Excluding(x => x.Location));
            character.Modifiers.Should().BeEquivalentTo(expectedCharacter.Modifiers, config => config.Excluding(x => x.Character));
        }

        [Test]
        public async Task GetForSummaryByOwnerIdAsync_LoadCharacterOwnedByUser_IncludingJobs()
        {
            TestDataUtil
                .AddUser();
            var user = TestDataUtil.GetLast<User>();

            TestDataUtil
                .AddJob()
                .AddOrigin();

            TestDataUtil.AddCharacter(user.Id, c => c.Jobs = new List<CharacterJob>
            {
                new CharacterJob
                {
                    JobId = TestDataUtil.GetLast<Job>().Id
                }
            });
            var expectedCharacter = TestDataUtil.GetLast<Character>();

            var characters = await _characterRepository.GetForSummaryByOwnerIdAsync(user.Id);

            var character = characters.First();
            character.Should().BeEquivalentTo(expectedCharacter, config => config.Excluding(x => x.Owner).Excluding(x => x.Jobs).Excluding(x => x.Origin));
            character.Jobs.First().Job.Should().BeEquivalentTo(TestDataUtil.GetLast<Job>());
            character.Origin.Should().BeEquivalentTo(TestDataUtil.GetLast<Origin>());
        }

        [Test]
        public async Task GetForSummaryByOwnerIdAsync_ShouldNotReturnsCharacterThatDoesNotBelongToTheUser()
        {
            TestDataUtil
                .AddJob()
                .AddOrigin();

            TestDataUtil.AddUser();
            var notOwnerUser = TestDataUtil.GetLast<User>();

            TestDataUtil.AddUser()
                .AddCharacter(TestDataUtil.GetLast<User>().Id);

            var characters = await _characterRepository.GetForSummaryByOwnerIdAsync(notOwnerUser.Id);
            characters.Should().BeEmpty();
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldSortResultByDateDescending()
        {
            TestDataUtil
                .AddUser()
                .AddCharacterWithRequiredDependencies()
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(300));

            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<Character>(0).Id, null,0, false);

            historyEntries.Select(h => h.Action).Should().BeEquivalentTo(
                TestDataUtil.Get<CharacterHistoryEntry>(1).Action,
                TestDataUtil.Get<CharacterHistoryEntry>(0).Action
            );
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldSortGroupAndCharacterResultByDateDescending()
        {
            TestDataUtil
                .AddUser()
                .AddLocation().AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<Group>())
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(200))
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(300));

            var character = TestDataUtil.Get<Character>(0);
            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId,0, false);

            historyEntries.Select(h => h.Action).Should().BeEquivalentTo(
                TestDataUtil.Get<CharacterHistoryEntry>(1).Action,
                TestDataUtil.Get<GroupHistoryEntry>(0).Action,
                TestDataUtil.Get<CharacterHistoryEntry>(0).Action
            );
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldNotLoadHistoryEntryThatDoesNotBelongToSpecifiedCharacter()
        {
            TestDataUtil
                .AddCharacterWithRequiredDependencies()
                .AddCharacterWithRequiredDependencies()
                .AddCharacterHistoryEntry();

            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<Character>(0).Id, null,0, false);

            historyEntries.Should().BeEmpty();
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldNotLoadGmEntryWhenGmFlagIsSet()
        {
            TestDataUtil
                .AddUser()
                .AddLocation().AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<Group>())
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

            var character = TestDataUtil.Get<Character>();
            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

            historyEntries.Should().BeEmpty();
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldLoadGmEntryTooWhenIsGmIsTrue()
        {
            TestDataUtil
                .AddUser()
                .AddLocation().AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<Group>())
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

            var character = TestDataUtil.Get<Character>();
            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, true);

            historyEntries.Should().HaveCount(4);
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldReturnsGroupHistory_MoreRecentThanTheLatestChararacterHistoryWhenRequestingPage0()
        {
            TestDataUtil
                .AddUser()
                .AddLocation().AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<Group>())
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(200));

            var character = TestDataUtil.Get<Character>();
            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

            historyEntries.Select(c => c.Action).Should().BeEquivalentTo(
                TestDataUtil.GetLast<GroupHistoryEntry>().Action,
                TestDataUtil.GetLast<CharacterHistoryEntry>().Action
            );
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldReturnsGroupHistoryThatHappenedBetweenCharacterActions()
        {
            TestDataUtil
                .AddUser()
                .AddLocation().AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<Group>())
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(400))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(200))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(300))
                .AddGroupHistoryEntry(h => h.Date = new DateTime(50))
                ;

            var character = TestDataUtil.Get<Character>();
            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

            historyEntries.Select(c => c.Action).Should().Contain(
                TestDataUtil.Get<GroupHistoryEntry>(1).Action,
                TestDataUtil.Get<GroupHistoryEntry>(0).Action
            );
        }


        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldNotReturnsEntryOfOtherGroups()
        {
            TestDataUtil
                .AddUser()
                .AddLocation().AddGroup()
                .AddGroupHistoryEntry(h => h.Date = new DateTime(200))
                .AddGroup()
                .AddCharacterWithRequiredDependencies(c => c.Group = TestDataUtil.GetLast<Group>())
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(100))
                .AddCharacterHistoryEntry(h => h.Date = new DateTime(400));

            var character = TestDataUtil.Get<Character>();

            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(character.Id, character.GroupId, 0, false);

            historyEntries.OfType<GroupHistoryEntry>().Should().BeEmpty();
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldReturnOnePageAtTheTime()
        {
            TestDataUtil
                .AddCharacterWithRequiredDependencies();
            for (var i = 0; i < 41; i++)
                TestDataUtil.AddCharacterHistoryEntry(h => h.Date = new DateTime(i * 100));

            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<Character>(0).Id, null,0, false);

            historyEntries.Should().HaveCount(40);
        }

        [Test]
        public async Task GetHistoryByCharacterIdAsync_ShouldReturnRequestedPage()
        {
            TestDataUtil
                .AddCharacterWithRequiredDependencies();
            for (var i = 0; i < 41; i++)
                TestDataUtil.AddCharacterHistoryEntry(h => h.Date = new DateTime(i * 100));

            var historyEntries = await _characterRepository.GetHistoryByCharacterIdAsync(TestDataUtil.Get<Character>(0).Id, null,1, false);

            historyEntries.Should().HaveCount(1);
            historyEntries.First().Action.Should().Be(TestDataUtil.Get<CharacterHistoryEntry>(0).Action);
        }
    }
}