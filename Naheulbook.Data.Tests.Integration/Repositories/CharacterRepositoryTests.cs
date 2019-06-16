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