using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
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

            var actualUser = await _userRepository.GetByUsernameAsync(expectedUser.Username!);

            actualUser.Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public async Task CanGetUserByFacebookId()
        {
            TestDataUtil.AddUser();
            var expectedUser = TestDataUtil.GetLast<User>();

            var actualUser = await _userRepository.GetByFacebookIdAsync(expectedUser.FbId!);

            actualUser.Should().BeEquivalentTo(TestDataUtil.GetLast<User>());
        }

        [Test]
        public async Task SearchUser_ShouldReturnsMatchingUsers()
        {
            TestDataUtil.AddUser(u => u.ShowInSearchUntil = RoundDate(DateTime.Now.AddDays(1)));
            var testUser = TestDataUtil.GetLast<User>();

            var users = await _userRepository.SearchUsersAsync(testUser.DisplayName!);

            users.Should().BeEquivalentTo(testUser);
        }

        [Test]
        public async Task SearchUser_ShouldNotDisplayUser_WhenShowInSearchUntilIsOlderThanNow()
        {
            TestDataUtil.AddUser(u => u.ShowInSearchUntil = DateTime.Now.AddDays(-1));

            var testUser = TestDataUtil.GetLast<User>();

            var users = await _userRepository.SearchUsersAsync(testUser.DisplayName!);

            users.Should().BeEmpty();
        }

        private static DateTime? RoundDate(DateTime date)
        {
            return date.AddNanoseconds(-date.Nanosecond());
        }
    }
}