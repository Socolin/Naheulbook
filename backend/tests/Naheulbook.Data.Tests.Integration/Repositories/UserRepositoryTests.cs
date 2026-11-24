using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using Naheulbook.Shared.Extensions;
using Naheulbook.TestUtils;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

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
        TestDataUtil.AddUser(out var user, u => u.AdminLevel = UserAdminLevel.Admin);

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

    #region GetByMicrosoftIdAsync

    #endregion

    #region SearchUsersAsync

    [Test]
    public async Task SearchUser_ShouldReturnsMatchingUsers()
    {
        TestDataUtil.AddUser(out var user, u => u.ShowInSearchUntil = DateTime.Now.AddDays(1).RoundToSeconds());

        var actualEntities = await _userRepository.SearchUsersAsync(user.DisplayName!);

        AssertEntitiesAreLoaded(actualEntities, [user]);
    }

    [Test]
    public async Task SearchUser_ShouldNotDisplayUser_WhenShowInSearchUntilIsOlderThanNow()
    {
        TestDataUtil.AddUser(out var user, u => u.ShowInSearchUntil = DateTime.Now.AddDays(-1).RoundToSeconds());

        var users = await _userRepository.SearchUsersAsync(user.DisplayName!);

        users.Should().BeEmpty();
    }

    #endregion

    #region SearchByIdentifiersAsync

    [Test]
    public async Task SearchByIdentifiersAsync_ShouldReturnUsersMatchingAllGivenIdentifiers()
    {
        TestDataUtil.AddUser(out var user, u => u.TwitterId = RngUtil.GetRandomHexString(8));
        TestDataUtil.AddUser();

        var byFacebookId = await _userRepository.SearchByIdentifiersAsync(new UserIdentifiersFilter(null, user.FbId, null, null, null));
        var byGoogleAndTwitterId = await _userRepository.SearchByIdentifiersAsync(new UserIdentifiersFilter(null, null, user.GoogleId, null, user.TwitterId));
        var byMicrosoftIdAndOtherUsername = await _userRepository.SearchByIdentifiersAsync(new UserIdentifiersFilter("some-other-username", null, null, user.MicrosoftId, null));

        AssertEntitiesAreLoaded(byFacebookId, [user]);
        AssertEntitiesAreLoaded(byGoogleAndTwitterId, [user]);
        byMicrosoftIdAndOtherUsername.Should().BeEmpty();
    }

    #endregion

    #region DeleteWithOwnedDataAsync

    [Test]
    public async Task DeleteWithOwnedDataAsync_ShouldDeleteUserAndOwnedData_AndKeepDataOfOtherUsers()
    {
        TestDataUtil
            .AddUser(out var deletedUser)
            .AddUserAccessToken()
            .AddOrigin()
            .AddItemTemplateAndRequiredData()
            .AddItemTemplate(out var privateItemTemplate, it =>
            {
                it.Source = "private";
                it.SourceUserId = deletedUser.Id;
            })
            // Group of the deleted user, with a character of another player
            .AddGroup(out var deletedGroup)
            .AddNpc()
            .AddLoot().AddItemToLoot(out var lootItem)
            .AddFight()
            .AddMonster(out var monster).AddItemToMonster(out var monsterItem, monster)
            .AddUser(out var otherUser)
            .AddCharacter(out var otherCharacter)
            .AddItemToCharacter(out var otherCharacterItem, otherCharacter)
            // Character of the deleted user, in the group of another GM
            .AddGroup(out var otherGroup)
            .AddCharacter(out var deletedCharacter, c => c.OwnerId = deletedUser.Id)
            .AddItemToCharacter(out var deletedCharacterItem, deletedCharacter)
            .AddJob(out var job).AddCharacterJob(job)
            .AddAptitudeGroup().AddAptitude().AddCharacterAptitude()
            .AddTalentGroup().AddTalent().AddCharacterTalent()
            .AddMasteryGroup().AddMastery().AddCharacterMastery()
            .AddHandicap().AddCharacterHandicap()
            .AddSpecialAttackGroup().AddSpecialAttack().AddCharacterSpecialAttack()
            .AddSpellBook().AddCharacterSpellBook()
            .AddCharacterModifier()
            .AddCharacterHistoryEntry()
            // Map layer of the deleted user, targeted by a link of an official marker
            .AddMap()
            .AddMapLayer(out var deletedMapLayer, l => l.UserId = deletedUser.Id)
            .AddMapMarker(out var deletedMapMarker)
            .AddMapMarkerLink(deletedMapMarker)
            .AddMapLayer()
            .AddMapMarker()
            .AddMapMarkerLink(out var otherMapMarkerLink, deletedMapMarker)
            ;

        await _userRepository.DeleteWithOwnedDataAsync(deletedUser.Id);

        await using var dbContext = DbUtils.GetTestDbContext<NaheulbookDbContext>();
        (await dbContext.Users.AnyAsync(u => u.Id == deletedUser.Id)).Should().BeFalse();
        (await dbContext.UserAccessTokens.AnyAsync(t => t.UserId == deletedUser.Id)).Should().BeFalse();
        (await dbContext.Groups.AnyAsync(g => g.Id == deletedGroup.Id)).Should().BeFalse();
        (await dbContext.Npcs.AnyAsync(n => n.GroupId == deletedGroup.Id)).Should().BeFalse();
        (await dbContext.Monsters.AnyAsync(m => m.GroupId == deletedGroup.Id)).Should().BeFalse();
        (await dbContext.Items.AnyAsync(i => i.Id == lootItem.Id || i.Id == monsterItem.Id || i.Id == deletedCharacterItem.Id)).Should().BeFalse();
        (await dbContext.Characters.AnyAsync(c => c.Id == deletedCharacter.Id)).Should().BeFalse();
        (await dbContext.MapLayers.AnyAsync(l => l.Id == deletedMapLayer.Id)).Should().BeFalse();
        (await dbContext.MapMarkers.AnyAsync(m => m.Id == deletedMapMarker.Id)).Should().BeFalse();

        (await dbContext.Users.AnyAsync(u => u.Id == otherUser.Id)).Should().BeTrue();
        (await dbContext.Groups.AnyAsync(g => g.Id == otherGroup.Id)).Should().BeTrue();
        (await dbContext.Characters.SingleAsync(c => c.Id == otherCharacter.Id)).GroupId.Should().BeNull();
        (await dbContext.Items.AnyAsync(i => i.Id == otherCharacterItem.Id)).Should().BeTrue();
        (await dbContext.MapMarkerLinks.SingleAsync(l => l.Id == otherMapMarkerLink.Id)).TargetMapMarkerId.Should().BeNull();
        (await dbContext.ItemTemplates.SingleAsync(it => it.Id == privateItemTemplate.Id)).SourceUserId.Should().BeNull();
    }

    #endregion
}
