using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class GroupRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private GroupRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new GroupRepository(RepositoryDbContext);
        }

        #region GetGroupsOwnedByAsync

        [Test]
        public async Task GetGroupsOwnedByAsync_ItShouldLoadGroupsWithCharacterList()
        {
            TestDataUtil
                .AddOrigin()
                .AddUser(out var user)
                .AddGroup(out var group1)
                .AddCharacter(out var character1)
                .AddCharacter(out var character2)
                .AddGroup(out var group2)
                .AddCharacter(out var character3)
                .AddCharacter(out var character4)
                ;

            var groups = await _repository.GetGroupsOwnedByAsync(user.Id);

            AssertEntitiesAreLoaded(groups, new[] {group1, group2});
            AssertEntitiesAreLoaded(groups.Single(x => x.Id == group1.Id).Characters, new[] {character1, character2});
            AssertEntitiesAreLoaded(groups.Single(x => x.Id == group2.Id).Characters, new[] {character3, character4});
        }

        [Test]
        public async Task GetGroupsOwnedByAsync_ItShouldNotReturnGroupNotOwnedByUser()
        {
            TestDataUtil
                .AddUser()
                .AddGroup()
                .AddUser();

            var notOwnerUser = TestDataUtil.GetLast<UserEntity>();

            var groups = await _repository.GetGroupsOwnedByAsync(notOwnerUser.Id);

            groups.Should().BeEmpty();
        }

        #endregion

        #region GetGroupsWithDetailsAsync

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldLoadGroupWithAllRequiredData()
        {
            TestDataUtil
                .AddJob(out var job)
                .AddOrigin(out var origin)
                .AddUser()
                .AddGroup(out var group)
                .AddCharacter(out var characterInGroup, c => c.GroupId = group.Id)
                .AddCharacter(out var invitedCharacter1, c => c.GroupId = null)
                .AddCharacterJob(out var characterJob1)
                .AddGroupInvite(out var groupInvite1, true)
                .AddCharacter(out var invitedCharacter2, c => c.GroupId = null)
                .AddCharacterJob(out var characterJob2)
                .AddGroupInvite(out var groupInvite2, false)
                ;

            var actualGroup = await _repository.GetGroupsWithDetailsAsync(group.Id);

            AssertEntityIsLoaded(actualGroup, group);
            AssertEntitiesAreLoaded(actualGroup.Characters, new[] {characterInGroup});
            AssertEntitiesAreLoaded(actualGroup.Invites, new[] {groupInvite1, groupInvite2});
            var actualInvitedCharacter1 = actualGroup.Invites.Single(x => x.CharacterId == invitedCharacter1.Id).Character;
            AssertEntityIsLoaded(actualInvitedCharacter1, invitedCharacter1);
            AssertEntityIsLoaded(actualInvitedCharacter1.Origin, origin);
            AssertEntitiesAreLoaded(actualInvitedCharacter1.Jobs, new [] {characterJob1});
            AssertEntityIsLoaded(actualInvitedCharacter1.Jobs.Single().Job, job);
            var actualInvitedCharacter2 = actualGroup.Invites.Single(x => x.CharacterId == invitedCharacter2.Id).Character;
            AssertEntityIsLoaded(actualInvitedCharacter2, invitedCharacter2);
            AssertEntityIsLoaded(actualInvitedCharacter2.Origin, origin);
            AssertEntitiesAreLoaded(actualInvitedCharacter2.Jobs, new [] {characterJob2});
            AssertEntityIsLoaded(actualInvitedCharacter2.Jobs.Single().Job, job);
        }

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldReturnsNullIfGroupDoesNotExists()
        {
            TestDataUtil
                .AddUser()
                .AddGroup(out var group);

            var actualGroup = await _repository.GetGroupsWithDetailsAsync(-1);
            var expectedGroup = await _repository.GetGroupsWithDetailsAsync(group.Id);

            actualGroup.Should().BeNull();
            expectedGroup.Should().NotBeNull();
        }

        #endregion

        #region GetGroupsWithCharactersAsync

        // FIXME

        #endregion
    }
}