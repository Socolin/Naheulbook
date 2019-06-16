using System;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils
{
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
        public void EnsureAdminAccess_WhenUserIsNotAdmin_Throw()
        {
            var user = new User {Admin = false, Id = 1};

            _unitOfWork.Users.GetAsync(1)
                .Returns(user);

            Func<Task> act = () => _authorizationUtil.EnsureAdminAccessAsync(new NaheulbookExecutionContext {UserId = 1});

            act.Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public void EnsureAdminAccess_WhenUserDoesNotExists_Throw()
        {
            _unitOfWork.Users.GetAsync(1)
                .Returns((User) null);

            Func<Task> act = () => _authorizationUtil.EnsureAdminAccessAsync(new NaheulbookExecutionContext {UserId = 1});

            act.Should().Throw<ForbiddenAccessException>();
        }


        [Test]
        public void EnsureCanEditItemTemplateAsync_WhenUserDoesNotExists_Throw()
        {
            var itemTemplate = new ItemTemplate {Source = "official"};
            var executionContext = new NaheulbookExecutionContext {UserId = 1};

            var user = new User {Admin = false, Id = 1};

            _unitOfWork.Users.GetAsync(1)
                .Returns(user);

            Func<Task> act = () => _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

            act.Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public void EnsureCanEditItemTemplateAsync_WhenSourceIsOfficial_ShouldThrowWhenUserIsNotAdmin()
        {
            var itemTemplate = new ItemTemplate {Source = "official"};
            _unitOfWork.Users.GetAsync(1)
                .Returns((User) null);

            Func<Task> act = () => _authorizationUtil.EnsureAdminAccessAsync(new NaheulbookExecutionContext {UserId = 1});

            act.Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        [TestCase("private")]
        [TestCase("community")]
        public void EnsureCanEditItemTemplateAsync_WhenSourceIsPrivateOrCommunity_ShouldThrowWhenUserIsNotTheSame(string source)
        {
            var itemTemplate = new ItemTemplate {Source = source, SourceUserId = 2};
            var executionContext = new NaheulbookExecutionContext {UserId = 1};

            var user = new User {Admin = false, Id = 1};

            _unitOfWork.Users.GetAsync(1)
                .Returns(user);

            Func<Task> act = () => _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

            act.Should().Throw<ForbiddenAccessException>();
        }


        [Test]
        public void EnsureCanEditItemTemplateAsync_WhenSourceIsInvalid_ShouldThrow()
        {
            var itemTemplate = new ItemTemplate {Source = "invalid"};
            var executionContext = new NaheulbookExecutionContext();

            Func<Task> act = () => _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

            act.Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public void EnsureIsGroupOwner_WhenUserIsDifferentFromGroupMasterId_ShouldThrow()
        {
            var group = new Group {MasterId = 10};
            var executionContext = new NaheulbookExecutionContext {UserId = 15};

            Action act = () => _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            act.Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public void EnsureIsGroupOwner_WhenUserIsSameFromGroupMasterId_ShouldNotThrow()
        {
            var group = new Group {MasterId = 15};
            var executionContext = new NaheulbookExecutionContext {UserId = 15};

            Action act = () => _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            act.Should().NotThrow();
        }

        [Test]
        public void EnsureCharacterAccess_WhenUserIsNotMasterOrOwner_ShouldThrow()
        {
            var character = new Character {OwnerId = 10, Group = new Group {MasterId = 15}};
            var executionContext = new NaheulbookExecutionContext {UserId = 5};

            Action act = () => _authorizationUtil.EnsureCharacterAccess(executionContext, character);

            act.Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public void EnsureCharacterAccess_WhenUserIsSameAsGroupMasterId_ShouldNotThrow()
        {
            var character = new Character {OwnerId = 10, Group = new Group {MasterId = 15}};
            var executionContext = new NaheulbookExecutionContext {UserId = 15};

            Action act = () => _authorizationUtil.EnsureCharacterAccess(executionContext, character);

            act.Should().NotThrow();
        }

        [Test]
        public void EnsureCharacterAccess_WhenUserIsSameAsOwnerID_ShouldNotThrow()
        {
            var character = new Character {OwnerId = 10, Group = new Group {MasterId = 15}};
            var executionContext = new NaheulbookExecutionContext {UserId = 10};

            Action act = () => _authorizationUtil.EnsureCharacterAccess(executionContext, character);

            act.Should().NotThrow();
        }
    }
}