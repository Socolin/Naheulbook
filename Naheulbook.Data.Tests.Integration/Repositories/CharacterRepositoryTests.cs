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

            var character = await _characterRepository.GetWithAllDataAsync(user.Id);

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
    }
}