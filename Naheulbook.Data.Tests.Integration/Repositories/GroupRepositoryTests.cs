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

        [Test]
        public async Task zGetGroupsOwnedByAsync_ItShouldLoadGroupsWithCharacterList()
        {
            TestDataUtil
                .AddUser()
                .AddGroup()
                .AddOrigin()
                .AddCharacter(c => c.GroupId = TestDataUtil.GetLast<GroupEntity>().Id);

            var groups = await _groupRepository.GetGroupsOwnedByAsync(TestDataUtil.GetLast<UserEntity>().Id);

            groups.Should().BeEquivalentTo(TestDataUtil.GetAll<GroupEntity>(), config => config.Excluding(g => g.Characters).Excluding(g => g.Master));
            groups.First().Characters.Should().BeEquivalentTo(TestDataUtil.GetAll<CharacterEntity>(), config => config.Excluding(c => c.Group).Excluding(c => c.Owner).Excluding(c => c.Origin));
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

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldLoadGroupWithAllRequiredData()
        {
            var expectedGroup = TestDataUtil.AddUser().AddGroup().GetLast<GroupEntity>();
            var expectedCharacter = TestDataUtil.AddOrigin().AddCharacter(c => c.GroupId = TestDataUtil.GetLast<GroupEntity>().Id).GetLast<CharacterEntity>();
            var expectedInvite1 = TestDataUtil.AddCharacter().AddGroupInvite(TestDataUtil.GetLast<CharacterEntity>(), TestDataUtil.GetLast<GroupEntity>(), true).GetLast<GroupInviteEntity>();
            var expectedInvite2 = TestDataUtil.AddCharacter().AddGroupInvite(TestDataUtil.GetLast<CharacterEntity>(), TestDataUtil.GetLast<GroupEntity>(), false).GetLast<GroupInviteEntity>();

            var group = await _groupRepository.GetGroupsWithDetailsAsync(expectedGroup.Id);

            group.Should().BeEquivalentTo(expectedGroup, config => config.Excluding(g => g.Characters).Excluding(g => g.Master).Excluding(g => g.Invites));
            group!.Characters.Should().BeEquivalentTo(new [] {expectedCharacter}, config => config.Excluding(c => c.Group).Excluding(c => c.Owner).Excluding(c => c.Origin));
            group!.Invites.Should().BeEquivalentTo(new [] {expectedInvite1, expectedInvite2}, config => config.Excluding(i => i.Character).Excluding(i => i.Group));
        }

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldReturnsNullIfGroupDoesNotExists()
        {
            var group = await _groupRepository.GetGroupsWithDetailsAsync(1000);

            group.Should().BeNull();
        }

    }
}