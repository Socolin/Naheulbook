using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemTemplateServiceTests
    {
        private IAuthorizationUtil _authorizationUtil;
        private ItemTemplateService _itemTemplateService;
        private IMapper _mapper;
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IItemTemplateUtil _itemTemplateUtil;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplateUtil = Substitute.For<IItemTemplateUtil>();
            _itemTemplateService = new ItemTemplateService(
                _unitOfWorkFactory,
                _authorizationUtil,
                _mapper,
                _itemTemplateUtil
            );
        }

        [Test]
        public async Task GetItemTemplateAsync_LoadItemTemplateFromDbWithFullData_AndReturnsIt()
        {
            var expectedItemTemplate = new ItemTemplate();

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(1)
                .Returns(expectedItemTemplate);

            var itemTemplate = await _itemTemplateService.GetItemTemplateAsync(1);

            itemTemplate.Should().Be(expectedItemTemplate);
        }

        [Test]
        public void GetItemTemplateAsync_WhenItemTemplateIsNotFound_Throw()
        {
            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(1)
                .Returns((ItemTemplate) null);

            Func<Task> act = () => _itemTemplateService.GetItemTemplateAsync(1);

            act.Should().Throw<ItemTemplateNotFoundException>();
        }

        [Test]
        public async Task CreateItemTemplate_AddANewItemTemplateInDatabase_AndReturnFullyLoadedOne()
        {
            var newItemTemplateEntity = new ItemTemplate {Id = 1};
            var fullyLoadedItemTemplate = new ItemTemplate {Id = 1};
            var createItemTemplateRequest = new ItemTemplateRequest();

            _mapper.Map<ItemTemplate>(createItemTemplateRequest)
                .Returns(newItemTemplateEntity);
            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(1)
                .Returns(fullyLoadedItemTemplate);

            var actualItemTemplate = await _itemTemplateService.CreateItemTemplateAsync(new NaheulbookExecutionContext(), createItemTemplateRequest);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.Add(newItemTemplateEntity);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });
            actualItemTemplate.Should().Be(fullyLoadedItemTemplate);
        }

        [Test]
        public async Task CreateItemTemplate_EnsureThatUserIsAdmin_IfSourceIsOfficial_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();
            var createItemTemplateRequest = new ItemTemplateRequest {Source = "official"};

            _authorizationUtil.EnsureAdminAccessAsync(executionContext)
                .Throws(new TestException());

            Func<Task<ItemTemplate>> act = () => _itemTemplateService.CreateItemTemplateAsync(executionContext, createItemTemplateRequest);

            act.Should().Throw<TestException>();
            await _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task CreateItemTemplate_SetSourceUserIdForNonOfficialItem()
        {
            var executionContext = new NaheulbookExecutionContext {UserId = 42};
            var createItemTemplateRequest = new ItemTemplateRequest {Source = "non-official"};
            var newItemTemplateEntity = new ItemTemplate {Id = 1};

            _mapper.Map<ItemTemplate>(createItemTemplateRequest)
                .Returns(newItemTemplateEntity);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(callInfo => newItemTemplateEntity.SourceUserId.Should().Be(42));

            await _itemTemplateService.CreateItemTemplateAsync(executionContext, createItemTemplateRequest);

            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public async Task EditItemTemplateAsync_LoadEntityWithRelatedStuff_ThenApplyChangesFromRequest_ThenSave()
        {
            const int itemTemplateId = 4;
            var itemTemplateRequest = new ItemTemplateRequest();
            var fullyLoadedItemTemplate = new ItemTemplate {Id = 1};

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
                .Returns(fullyLoadedItemTemplate);

            await _itemTemplateService.EditItemTemplateAsync(new NaheulbookExecutionContext(), itemTemplateId, itemTemplateRequest);

            Received.InOrder(() =>
            {
                _itemTemplateUtil.ApplyChangesFromRequest(fullyLoadedItemTemplate, itemTemplateRequest);
                _unitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });
        }

        [Test]
        public async Task EditItemTemplateAsync_ShouldReplaceSourceUserId_IfSourceIsNotOfficial()
        {
            const int itemTemplateId = 4;
            const int userId = 17;
            var itemTemplateRequest = new ItemTemplateRequest {Source = "private"};
            var fullyLoadedItemTemplate = new ItemTemplate {Id = 1};
            var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = userId};

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
                .Returns(fullyLoadedItemTemplate);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => fullyLoadedItemTemplate.SourceUserId.Should().Be(userId));

            await _itemTemplateService.EditItemTemplateAsync(naheulbookExecutionContext, itemTemplateId, itemTemplateRequest);

            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public async Task EditItemTemplateAsync_ShouldSetNullSourceUserId_IfSourceIsOfficial()
        {
            const int itemTemplateId = 4;
            var itemTemplateRequest = new ItemTemplateRequest {Source = "official"};
            var fullyLoadedItemTemplate = new ItemTemplate {Id = 1};
            var naheulbookExecutionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
                .Returns(fullyLoadedItemTemplate);
            _unitOfWorkFactory.GetUnitOfWork().When(x => x.CompleteAsync())
                .Do(info => fullyLoadedItemTemplate.SourceUserId.Should().BeNull());

            await _itemTemplateService.EditItemTemplateAsync(naheulbookExecutionContext, itemTemplateId, itemTemplateRequest);

            await _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
        }

        [Test]
        public void EditItemTemplateAsync_CheckPermissionBeforeEditingItem()
        {
            const int itemTemplateId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var fullyLoadedItemTemplate = new ItemTemplate {Id = 1};

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
                .Returns(fullyLoadedItemTemplate);
            _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, fullyLoadedItemTemplate)
                .Returns(Task.FromException(new TestException()));

            Func<Task<ItemTemplate>> act = () => _itemTemplateService.EditItemTemplateAsync(executionContext, itemTemplateId, new ItemTemplateRequest());

            using (new AssertionScope())
            {
                act.Should().Throw<TestException>();
                _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
            }
        }


        [Test]
        public void EditItemTemplateAsync_CheckPermissionBeforeChangingSourceOfAnItemToAdmin()
        {
            const int itemTemplateId = 4;
            var executionContext = new NaheulbookExecutionContext();
            var request = new ItemTemplateRequest {Source = "official"};
            var fullyLoadedItemTemplate = new ItemTemplate {Id = 1, Source = "private"};

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
                .Returns(fullyLoadedItemTemplate);
            _authorizationUtil.EnsureAdminAccessAsync(executionContext)
                .Returns(Task.FromException(new TestException()));

            Func<Task<ItemTemplate>> act = () => _itemTemplateService.EditItemTemplateAsync(executionContext, itemTemplateId, request);

            using (new AssertionScope())
            {
                act.Should().Throw<TestException>();
                _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
            }
        }
    }
}