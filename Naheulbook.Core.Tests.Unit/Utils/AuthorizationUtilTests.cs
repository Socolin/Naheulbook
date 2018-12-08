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
    }
}