using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Naheulbook.Core.Tests.Unit.Services;

[TestFixture]
[TestOf(typeof(MerchantService))]
public class MerchantServiceTest
{
    private MerchantService _service;

    private NaheulbookExecutionContext _executionContext;
    private FakeUnitOfWorkFactory _unitOfWorkFactory;
    private INotificationSessionFactory _notificationSessionFactory;
    private IMerchantFactory _merchantFactory;
    private IAuthorizationUtil _authorizationUtil;

    [SetUp]
    public void SetUp()
    {
        _executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory = new FakeUnitOfWorkFactory();
        _notificationSessionFactory = Substitute.For<INotificationSessionFactory>();
        _merchantFactory = Substitute.For<IMerchantFactory>();
        _authorizationUtil = Substitute.For<IAuthorizationUtil>();

        _service = new MerchantService(
            _unitOfWorkFactory,
            _notificationSessionFactory,
            _merchantFactory,
            _authorizationUtil
        );
    }

    [Test]
    public async Task CreateAsync_CreateANewMerchant_AndSaveIdToTheDatabase_AndNotifyChanges_ThenReturnTheCreatedMerchant()
    {
        const int groupId = 1;
        var request = new CreateMerchantRequest {Name = "some-name"};
        var notificationSession = Substitute.For<INotificationSession>();
        var merchant = new MerchantEntity();
        var group = new GroupEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _merchantFactory.Create(groupId, request)
            .Returns(merchant);
        _notificationSessionFactory.CreateSession()
            .Returns(notificationSession);

        var actual = await _service.CreateAsync(_executionContext, groupId, request);

        actual.Should().BeSameAs(merchant);

        Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().Merchants.Add(merchant);
                notificationSession.NotifyGroupAddMerchant(groupId, merchant);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
                notificationSession.CommitAsync();
            }
        );
    }

    [Test]
    public async Task CreateAsync_ShouldEnsureAccess()
    {
        const int groupId = 1;
        var group = new GroupEntity();

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns(group);
        _authorizationUtil.When(x => x.EnsureIsGroupOwner(_executionContext, group))
            .Throw(new TestException());

        var act = () => _service.CreateAsync(_executionContext, groupId, new CreateMerchantRequest {Name = "some-name"});

        await act.Should().ThrowAsync<TestException>();
    }

    [Test]
    public async Task CreateAsync_WhenTheGroupDoesNotExists_Throw()
    {
        const int groupId = 1;

        _unitOfWorkFactory.GetUnitOfWork().Groups.GetAsync(groupId)
            .Returns((GroupEntity)null);

        var act = () => _service.CreateAsync(_executionContext, groupId, new CreateMerchantRequest {Name = "some-name"});

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}