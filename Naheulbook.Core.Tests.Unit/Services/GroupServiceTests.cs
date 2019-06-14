using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class GroupServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private GroupService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();

            _service = new GroupService(_unitOfWorkFactory);
        }

        [Test]
        public async Task CreateGroupAsync_CreateANewGroupInDatabase_SettingMasterToCurrentUserId()
        {
            var createGroupRequest = new CreateGroupRequest {Name = "some-name"};
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = 10};
            var location = new Location();

            _unitOfWorkFactory.GetUnitOfWork().Locations.GetNewGroupDefaultLocationAsync()
                .Returns(location);

            var actualGroup = await _service.CreateGroupAsync(naheulbookExecutionContext, createGroupRequest);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().Groups.Add(actualGroup);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });

            actualGroup.Location.Should().BeSameAs(location);
            actualGroup.Name.Should().Be("some-name");
            actualGroup.MasterId.Should().Be(10);
        }
    }
}