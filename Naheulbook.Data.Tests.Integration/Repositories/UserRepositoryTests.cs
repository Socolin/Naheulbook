using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class UserRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private UserRepository _userRepository;

        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository(RepositoryDbContext);
        }

        [Test]
        public async Task CanGetUserByUsername()
        {
            var user = new User()
            {
                Username = "some-username",
                DisplayName = "some-display-name",
                ActivationCode = "some-activation-code",
                Admin = true,
                HashedPassword = "some-hashed-password",
                FbId = "some-fb-id",
                GoogleId = "some-google-id",
                TwitterId = "some-twitter-id",
            };

            await AddInDbAsync(user);

            var actualUser = await _userRepository.GetByUsernameAsync("some-username");

            actualUser.Should().BeEquivalentTo(user);
        }
    }
}