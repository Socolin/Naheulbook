using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class EventServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IAuthorizationUtil _authorizationUtil;
        private EventService _service;


        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();

            _service = new EventService(
                _unitOfWorkFactory,
                _authorizationUtil
            );
        }

        [Test]
        public async Task GetEventsForGroupAsync_ShouldLoadEventsListAndReturnIt()
        {
            const int groupId = 42;
            var executionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};
            var expectedEvents = new List<Event>();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);
            _unitOfWorkFactory.GetUnitOfWork().Events.GetByGroupIdAsync(groupId)
                .Returns(expectedEvents);

            var events = await _service.GetEventsForGroupAsync(executionContext, groupId);

            events.Should().BeSameAs(expectedEvents);
        }

        [Test]
        public async Task GetEventsForGroupAsync_ShouldThrowWhenGroupNotFound()
        {
            const int groupId = 42;
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns((Group) null);

            Func<Task> act = () => _service.GetEventsForGroupAsync(executionContext, groupId);

            await act.Should().ThrowAsync<GroupNotFoundException>();
        }

        [Test]
        public async Task GetEventsForGroupAsync_ShouldEnsureGroupAccess()
        {
            const int groupId = 42;
            var naheulbookExecutionContext = new NaheulbookExecutionContext();
            var group = new Group {Id = groupId};

            _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
                .Returns(group);

            _authorizationUtil.When(x => x.EnsureIsGroupOwner(naheulbookExecutionContext, group))
                .Throw(new TestException());

            Func<Task> act = () => _service.GetEventsForGroupAsync(naheulbookExecutionContext, groupId);

            await act.Should().ThrowAsync<TestException>();
        }
    }
}