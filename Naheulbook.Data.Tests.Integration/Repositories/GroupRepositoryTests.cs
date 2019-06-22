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
        public async Task GetGroupsOwnedByAsync_ItShouldLoadGroupsWithCharacterList()
        {
            TestDataUtil
                .AddUser()
                .AddLocation()
                .AddGroup()
                .AddOrigin()
                .AddCharacter(c => c.GroupId = TestDataUtil.GetLast<Group>().Id);

            var groups = await _groupRepository.GetGroupsOwnedByAsync(TestDataUtil.GetLast<User>().Id);

            groups.Should().BeEquivalentTo(TestDataUtil.GetAll<Group>(), config => config.Excluding(g => g.Characters).Excluding(g => g.Master).Excluding(g => g.Location));
            groups.First().Characters.Should().BeEquivalentTo(TestDataUtil.GetAll<Character>(), config => config.Excluding(c => c.Group).Excluding(c => c.Owner).Excluding(c => c.Origin));
        }

        [Test]
        public async Task GetGroupsOwnedByAsync_ItShouldNotReturnGroupNotOwnedByUser()
        {
            TestDataUtil
                .AddLocation()
                .AddUser()
                .AddGroup()
                .AddUser();

            var notOwnerUser = TestDataUtil.GetLast<User>();

            var groups = await _groupRepository.GetGroupsOwnedByAsync(notOwnerUser.Id);

            groups.Should().BeEmpty();
        }

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldLoadGroupWithAllRequiredData()
        {
            var expectedLocation = TestDataUtil.AddLocation().GetLast<Location>();
            var expectedGroup = TestDataUtil.AddUser().AddGroup().GetLast<Group>();
            var expectedCharacter = TestDataUtil.AddOrigin().AddCharacter(c => c.GroupId = TestDataUtil.GetLast<Group>().Id).GetLast<Character>();
            var expectedInvite1 = TestDataUtil.AddCharacter().AddGroupInvite(TestDataUtil.GetLast<Character>(), TestDataUtil.GetLast<Group>(), true).GetLast<GroupInvite>();
            var expectedInvite2 = TestDataUtil.AddCharacter().AddGroupInvite(TestDataUtil.GetLast<Character>(), TestDataUtil.GetLast<Group>(), false).GetLast<GroupInvite>();

            var group = await _groupRepository.GetGroupsWithDetailsAsync(expectedGroup.Id);

            group.Should().BeEquivalentTo(expectedGroup, config => config.Excluding(g => g.Characters).Excluding(g => g.Master).Excluding(g => g.Location).Excluding(g => g.Invites));
            group.Characters.Should().BeEquivalentTo(new [] {expectedCharacter}, config => config.Excluding(c => c.Group).Excluding(c => c.Owner).Excluding(c => c.Origin));
            group.Location.Should().BeEquivalentTo(expectedLocation);
            group.Invites.Should().BeEquivalentTo(new [] {expectedInvite1, expectedInvite2}, config => config.Excluding(i => i.Character).Excluding(i => i.Group));
        }

        [Test]
        public async Task GetGroupsWithDetailsAsync_ShouldReturnsNullIfGroupDoesNotExists()
        {
            var group = await _groupRepository.GetGroupsWithDetailsAsync(1000);

            group.Should().BeNull();
        }

    }
}