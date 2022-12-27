using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Naheulbook.Data.DbContexts;
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

        #region GetByUsernameAsync

        [Test]
        public async Task CanGetUserByUsername()
        {
            TestDataUtil.AddUser(out var user, u => u.Admin = true);

            var actualUser = await _userRepository.GetByUsernameAsync(user.Username!);

            AssertEntityIsLoaded(actualUser, user);
        }

        #endregion

        #region GetByFacebookIdAsync

        [Test]
        public async Task CanGetUserByFacebookId()
        {
            TestDataUtil.AddUser(out var user);

            var actualUser = await _userRepository.GetByFacebookIdAsync(user.FbId!);

            AssertEntityIsLoaded(actualUser, user);
        }

        #endregion

        #region GetByGoogleIdAsync

        #endregion

        #region GetByTwitterIdAsync

        #endregion

        #region GetByMicrosoftIdAsync

        #endregion

        #region SearchUsersAsync

        [Test]
        public async Task SearchUser_ShouldReturnsMatchingUsers()
        {
            TestDataUtil.AddUser(out var user, u => u.ShowInSearchUntil = RoundDate(DateTime.Now.AddDays(1)));

            var actualEntities = await _userRepository.SearchUsersAsync(user.DisplayName!);

            AssertEntitiesAreLoaded(actualEntities, new[] {user});
        }

        [Test]
        public async Task SearchUser_ShouldNotDisplayUser_WhenShowInSearchUntilIsOlderThanNow()
        {
            TestDataUtil.AddUser(out var user, u => u.ShowInSearchUntil = DateTime.Now.AddDays(-1));

            var users = await _userRepository.SearchUsersAsync(user.DisplayName!);

            users.Should().BeEmpty();
        }

        #endregion


        private static DateTime? RoundDate(DateTime date)
        {
            return date.AddNanoseconds(-date.Nanosecond());
        }
    }
}