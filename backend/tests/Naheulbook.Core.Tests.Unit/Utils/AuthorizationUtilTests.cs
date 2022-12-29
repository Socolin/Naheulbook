using System;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Extensions.UnitOfWorks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils;

public class AuthorizationUtilTests
{
    private IUserRepository _userRepository;
    private IUnitOfWork _unitOfWork;
    private AuthorizationUtil _authorizationUtil;

    [SetUp]
    public void SetUp()
    {
        var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork.Users.Returns(_userRepository);
        _authorizationUtil = new AuthorizationUtil(unitOfWorkFactory);
    }

    [Test]
    public async Task EnsureAdminAccess_WhenUserIsNotAdmin_Throw()
    {
        var user = new UserEntity {Admin = false, Id = 1};

        _unitOfWork.Users.GetAsync(1)
            .Returns(user);

        Func<Task> act = () => _authorizationUtil.EnsureAdminAccessAsync(new NaheulbookExecutionContext {UserId = 1});

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public async Task EnsureAdminAccess_WhenUserDoesNotExists_Throw()
    {
        _unitOfWork.Users.GetAsync(1)
            .Returns((UserEntity) null);

        Func<Task> act = () => _authorizationUtil.EnsureAdminAccessAsync(new NaheulbookExecutionContext {UserId = 1});

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }


    [Test]
    public async Task EnsureCanEditItemTemplateAsync_WhenUserDoesNotExists_Throw()
    {
        var itemTemplate = new ItemTemplateEntity {Source = "official"};
        var executionContext = new NaheulbookExecutionContext {UserId = 1};

        var user = new UserEntity {Admin = false, Id = 1};

        _unitOfWork.Users.GetAsync(1)
            .Returns(user);

        Func<Task> act = () => _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public async Task EnsureCanEditItemTemplateAsync_WhenSourceIsOfficial_ShouldThrowWhenUserIsNotAdmin()
    {
        _unitOfWork.Users.GetAsync(1)
            .Returns((UserEntity) null);

        Func<Task> act = () => _authorizationUtil.EnsureAdminAccessAsync(new NaheulbookExecutionContext {UserId = 1});

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    [TestCase("private")]
    [TestCase("community")]
    public async Task EnsureCanEditItemTemplateAsync_WhenSourceIsPrivateOrCommunity_ShouldThrowWhenUserIsNotTheSame(string source)
    {
        var itemTemplate = new ItemTemplateEntity {Source = source, SourceUserId = 2};
        var executionContext = new NaheulbookExecutionContext {UserId = 1};

        var user = new UserEntity {Admin = false, Id = 1};

        _unitOfWork.Users.GetAsync(1)
            .Returns(user);

        Func<Task> act = () => _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }


    [Test]
    public async Task EnsureCanEditItemTemplateAsync_WhenSourceIsInvalid_ShouldThrow()
    {
        var itemTemplate = new ItemTemplateEntity {Source = "invalid"};
        var executionContext = new NaheulbookExecutionContext();

        Func<Task> act = () => _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureIsGroupOwner_WhenUserIsDifferentFromGroupMasterId_ShouldThrow()
    {
        var group = new GroupEntity {MasterId = 10};
        var executionContext = new NaheulbookExecutionContext {UserId = 15};

        Action act = () => _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureIsGroupOwner_WhenUserIsSameFromGroupMasterId_ShouldNotThrow()
    {
        var group = new GroupEntity {MasterId = 15};
        var executionContext = new NaheulbookExecutionContext {UserId = 15};

        Action act = () => _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCharacterAccess_WhenUserIsNotMasterOrOwner_ShouldThrow()
    {
        var character = new CharacterEntity {OwnerId = 10, GroupId = 8, Group = new GroupEntity {MasterId = 15}};
        var executionContext = new NaheulbookExecutionContext {UserId = 5};

        Action act = () => _authorizationUtil.EnsureCharacterAccess(executionContext, character);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureCharacterAccess_WhenUserIsNotOwnerAndCharacterDoNotHaveAGroup()
    {
        var character = new CharacterEntity {OwnerId = 10, GroupId = null};
        var executionContext = new NaheulbookExecutionContext {UserId = 15};

        Action act = () => _authorizationUtil.EnsureCharacterAccess(executionContext, character);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCharacterAccess_WhenUserIsSameAsGroupMasterId_ShouldNotThrow()
    {
        var character = new CharacterEntity {OwnerId = 10, GroupId = 8, Group = new GroupEntity {MasterId = 15}};
        var executionContext = new NaheulbookExecutionContext {UserId = 15};

        Action act = () => _authorizationUtil.EnsureCharacterAccess(executionContext, character);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCharacterAccess_WhenUserIsSameAsOwnerID_ShouldNotThrow()
    {
        var character = new CharacterEntity {OwnerId = 10, GroupId = 8, Group = new GroupEntity {MasterId = 15}};
        var executionContext = new NaheulbookExecutionContext {UserId = 10};

        Action act = () => _authorizationUtil.EnsureCharacterAccess(executionContext, character);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureItemAccess_WhenItemIsOwnedByACharacter_ShouldThrowIfNotUserIsNotOwnerAndGroupMaster()
    {
        var item = new ItemEntity {Character = new CharacterEntity {OwnerId = 10, GroupId = 8, Group = new GroupEntity {MasterId = 15}}};
        var executionContext = new NaheulbookExecutionContext {UserId = 12};

        Action act = () => _authorizationUtil.EnsureItemAccess(executionContext, item);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureItemAccess_WhenItemIsOwnedByACharacter_ShouldNotThrowIfUserIsCharacterOwner()
    {
        var item = new ItemEntity {CharacterId = 1, Character = new CharacterEntity {OwnerId = 10, GroupId = 8, Group = new GroupEntity {MasterId = 15}}};
        var executionContext = new NaheulbookExecutionContext {UserId = 10};

        Action act = () => _authorizationUtil.EnsureItemAccess(executionContext, item);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureItemAccess_WhenItemIsOwnedByACharacter_ShouldNotThrowIfUserIsGroupMaster()
    {
        var item = new ItemEntity {CharacterId = 1, Character = new CharacterEntity {OwnerId = 10, GroupId = 8, Group = new GroupEntity {MasterId = 15}}};
        var executionContext = new NaheulbookExecutionContext {UserId = 15};

        Action act = () => _authorizationUtil.EnsureItemAccess(executionContext, item);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureItemAccess_WhenItemIsOwnedByALoot_ShouldThrowIfNotUserIsNotOwnerAndGroupMaster()
    {
        var item = new ItemEntity {LootId = 1, Loot = new LootEntity {GroupId = 8, Group = new GroupEntity {MasterId = 15}}};
        var executionContext = new NaheulbookExecutionContext {UserId = 12};

        Action act = () => _authorizationUtil.EnsureItemAccess(executionContext, item);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureItemAccess_WhenItemIsOwnedByALoot_ShouldNotThrowIfUserIsLootOwner()
    {
        var item = new ItemEntity {LootId = 1, Loot = new LootEntity {GroupId = 8, Group = new GroupEntity {MasterId = 15}}};
        var executionContext = new NaheulbookExecutionContext {UserId = 15};

        Action act = () => _authorizationUtil.EnsureItemAccess(executionContext, item);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureItemAccess_WhenItemIsOwnedByAMonster_ShouldThrowIfNotUserIsNotOwnerAndGroupMaster()
    {
        var item = new ItemEntity {MonsterId = 1, Monster = new MonsterEntity {GroupId = 8, Group = new GroupEntity {MasterId = 15}}};
        var executionContext = new NaheulbookExecutionContext {UserId = 12};

        Action act = () => _authorizationUtil.EnsureItemAccess(executionContext, item);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureItemAccess_WhenItemIsOwnedByAMonster_ShouldNotThrowIfUserIsMonsterOwner()
    {
        var item = new ItemEntity {MonsterId = 1, Monster = new MonsterEntity {GroupId = 8, Group = new GroupEntity {MasterId = 15}}};
        var executionContext = new NaheulbookExecutionContext {UserId = 15};

        Action act = () => _authorizationUtil.EnsureItemAccess(executionContext, item);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCanDeleteGroupInvite_ShouldThrowForbiddenException_WhenUserDoesNotMatchOwnerOfGroupOrCharacter()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 3};

        Action act = () => _authorizationUtil.EnsureCanDeleteGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureCanDeleteGroupInvite_WhenUserIsCharacterOwner_ShouldNotThrow()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 2};

        Action act = () => _authorizationUtil.EnsureCanDeleteGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCanDeleteGroupInvite_WhenUserIsGroupMaster_ShouldNotThrow()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 1};

        Action act = () => _authorizationUtil.EnsureCanDeleteGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCanAcceptGroupInvite_ShouldThrowWhenNotAuthorized()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 3};

        Action act = () => _authorizationUtil.EnsureCanAcceptGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureCanAcceptGroupInvite_ShouldThrowWhenInviteIsFromCharacter_AndUserIsCharacterOwner()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
            FromGroup = false,
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 2};

        Action act = () => _authorizationUtil.EnsureCanAcceptGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureCanAcceptGroupInvite_ShouldThrowWhenInviteIsFromGroup_AndUserIsGroupMaster()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
            FromGroup = true,
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 1};

        Action act = () => _authorizationUtil.EnsureCanAcceptGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureCanAcceptGroupInvite_ShouldNotThrowWhenInviteIsFromCharacter_AndUserIsGroupMaster()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
            FromGroup = false,
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 1};

        Action act = () => _authorizationUtil.EnsureCanAcceptGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCanAcceptGroupInvite_ShouldNotThrowWhenInviteIsFromGroup_AndUserIsCharacterOwner()
    {
        var groupInvite = new GroupInviteEntity
        {
            Group = new GroupEntity {MasterId = 1},
            Character = new CharacterEntity {OwnerId = 2},
            FromGroup = true,
        };
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 2};

        Action act = () => _authorizationUtil.EnsureCanAcceptGroupInvite(naheulbookExecutionContext, groupInvite);

        act.Should().NotThrow();
    }

    [Test]
    public void EnsureCanEditUser_ShouldThrowIfCurrentUserIdIsNotUserId()
    {
        var user = new UserEntity {Id = 3};
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 2};

        Action act = () => _authorizationUtil.EnsureCanEditUser(naheulbookExecutionContext, user);

        act.Should().Throw<ForbiddenAccessException>();
    }

    [Test]
    public void EnsureCanEditUser_ShouldNotThrowIfCurrentUserIdIsUserId()
    {
        var user = new UserEntity {Id = 2};
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 2};

        Action act = () => _authorizationUtil.EnsureCanEditUser(naheulbookExecutionContext, user);

        act.Should().NotThrow();
    }
}