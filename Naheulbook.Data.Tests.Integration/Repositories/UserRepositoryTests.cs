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
            TestDataUtil.AddUser(u => u.Admin = true);
            var expectedUser = TestDataUtil.GetLast<User>();

            var actualUser = await _userRepository.GetByUsernameAsync(expectedUser.Username);

            actualUser.Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public async Task CanGetUserByFacebookId()
        {
            TestDataUtil.AddUser();
            var expectedUser = TestDataUtil.GetLast<User>();

            var actualUser = await _userRepository.GetByFacebookIdAsync(expectedUser.FbId);

            actualUser.Should().BeEquivalentTo(TestDataUtil.GetLast<User>());

        }
    }
}