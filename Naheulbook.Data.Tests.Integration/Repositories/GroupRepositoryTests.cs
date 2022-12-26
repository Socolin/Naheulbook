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
        private GroupRepository _groupRepository;

        [SetUp]
        public void SetUp()
        {
            _groupRepository = new GroupRepository(RepositoryDbContext);
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

            var groups = await _groupRepository.GetGroupsOwnedByAsync(user.Id);

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

            var groups = await _groupRepository.GetGroupsOwnedByAsync(notOwnerUser.Id);

            groups.Should().BeEmpty();
        }

        #endregion

        #region GetGroupsWithDetailsAsync

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldLoadGroupWithAllRequiredData()
        {
            TestDataUtil
                .AddUser()
                .AddGroup(out var group)
                .AddOrigin()
                .AddCharacter(out var character, c => c.GroupId = group.Id)
                .AddCharacter(c => c.GroupId = null)
                .AddGroupInvite(out var groupInvite1, true)
                .AddCharacter(c => c.GroupId = null)
                .AddGroupInvite(out var groupInvite2, false)
                ;

            var actualGroup = await _groupRepository.GetGroupsWithDetailsAsync(group.Id);

            AssertEntityIsLoaded(actualGroup, group);
            AssertEntitiesAreLoaded(actualGroup.Characters, new[] {character});
            AssertEntitiesAreLoaded(actualGroup.Invites, new[] {groupInvite1, groupInvite2});
        }

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldReturnsNullIfGroupDoesNotExists()
        {
            TestDataUtil
                .AddUser()
                .AddGroup(out var group);

            var actualGroup = await _groupRepository.GetGroupsWithDetailsAsync(-1);
            var expectedGroup = await _groupRepository.GetGroupsWithDetailsAsync(group.Id);

            actualGroup.Should().BeNull();
            expectedGroup.Should().NotBeNull();
        }

        #endregion
    }
}