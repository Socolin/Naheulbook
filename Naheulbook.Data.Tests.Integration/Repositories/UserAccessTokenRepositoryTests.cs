using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class UserAccessTokenRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private UserAccessTokenRepository _userAccessTokenRepository;

        [SetUp]
        public void SetUp()
        {
            _userAccessTokenRepository = new UserAccessTokenRepository(RepositoryDbContext);
        }

        [Test]
        public async Task CanCreateUserAccessToken()
        {
            var user = TestDataUtil
                .AddUser().GetLast<UserEntity>();

            _userAccessTokenRepository.Add(CreateUserAccessToken(user));
            await RepositoryDbContext.SaveChangesAsync();
        }

        [Test]
        public async Task CanListUserTokens()
        {
            TestDataUtil
                .AddUser()
                .AddUserAccessToken();

            var tokens = await _userAccessTokenRepository.GetUserAccessTokensForUser(TestDataUtil.GetLast<UserEntity>().Id);

            tokens.Should().HaveCount(1);
            tokens.First().Should().BeEquivalentTo(TestDataUtil.GetLast<UserAccessTokenEntity>(), config => config.Excluding(x => x.User).Excluding(x => x.DateCreated));
        }

        private static UserAccessTokenEntity CreateUserAccessToken(UserEntity user)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Key = "some-random-key",
                DateCreated = DateTimeOffset.Now.ToUniversalTime(),
                UserId = user.Id,
                Name = "some-name"
            };
        }
    }
}