using System;
using System.Collections.Generic;
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

namespace Naheulbook.Core.Tests.Unit.Services;

public class ItemTemplateServiceTests
{
    private IAuthorizationUtil _authorizationUtil;
    private IMapper _mapper;
    private FakeUnitOfWorkFactory _unitOfWorkFactory;
    private IItemTemplateUtil _itemTemplateUtil;
    private IStringCleanupUtil _stringCleanupUtil;
    private ItemTemplateService _service;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkFactory = new FakeUnitOfWorkFactory();
        _authorizationUtil = Substitute.For<IAuthorizationUtil>();
        _mapper = Substitute.For<IMapper>();
        _itemTemplateUtil = Substitute.For<IItemTemplateUtil>();
        _stringCleanupUtil = Substitute.For<IStringCleanupUtil>();
        _service = new ItemTemplateService(
            _unitOfWorkFactory,
            _authorizationUtil,
            _mapper,
            _itemTemplateUtil,
            _stringCleanupUtil
        );
    }

    [Test]
    public async Task GetItemTemplateAsync_LoadItemTemplateFromDbWithFullData_AndReturnsIt()
    {
        var itemTemplateId = Guid.NewGuid();
        var expectedItemTemplate = new ItemTemplateEntity();

        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns(expectedItemTemplate);

        var itemTemplate = await _service.GetItemTemplateAsync(itemTemplateId);

        itemTemplate.Should().Be(expectedItemTemplate);
    }

    [Test]
    public async Task GetItemTemplateAsync_WhenItemTemplateIsNotFound_Throw()
    {
        var itemTemplateId = Guid.NewGuid();
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns((ItemTemplateEntity) null);

        Func<Task> act = () => _service.GetItemTemplateAsync(itemTemplateId);

        await act.Should().ThrowAsync<ItemTemplateNotFoundException>();
    }

    [Test]
    public async Task CreateItemTemplate_AddANewItemTemplateInDatabase_AndReturnFullyLoadedOne()
    {
        var itemTemplateId = Guid.NewGuid();
        var newItemTemplateEntity = new ItemTemplateEntity {Id = itemTemplateId};
        var fullyLoadedItemTemplate = new ItemTemplateEntity {Id = itemTemplateId};
        var createItemTemplateRequest = new ItemTemplateRequest();

        _mapper.Map<ItemTemplateEntity>(createItemTemplateRequest)
            .Returns(newItemTemplateEntity);
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns(fullyLoadedItemTemplate);

        var actualItemTemplate = await _service.CreateItemTemplateAsync(new NaheulbookExecutionContext(), createItemTemplateRequest);

        var itemTemplateRepository = _unitOfWorkFactory.GetUnitOfWork().ItemTemplates;
        Received.InOrder(() =>
        {
            itemTemplateRepository.Add(newItemTemplateEntity);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
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

        Func<Task<ItemTemplateEntity>> act = () => _service.CreateItemTemplateAsync(executionContext, createItemTemplateRequest);

        await act.Should().ThrowAsync<TestException>();
        await _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
    }

    [Test]
    public async Task CreateItemTemplate_SetSourceUserIdForNonOfficialItem()
    {
        var itemTemplateId = Guid.NewGuid();
        var executionContext = new NaheulbookExecutionContext {UserId = 42};
        var createItemTemplateRequest = new ItemTemplateRequest {Source = "non-official"};
        var newItemTemplateEntity = new ItemTemplateEntity {Id = itemTemplateId};

        _mapper.Map<ItemTemplateEntity>(createItemTemplateRequest)
            .Returns(newItemTemplateEntity);
        _unitOfWorkFactory.GetUnitOfWork().When(x => x.SaveChangesAsync())
            .Do(_ => newItemTemplateEntity.SourceUserId.Should().Be(42));

        await _service.CreateItemTemplateAsync(executionContext, createItemTemplateRequest);

        await _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
    }

    [Test]
    public async Task EditItemTemplateAsync_LoadEntityWithRelatedStuff_ThenApplyChangesFromRequest_ThenSave()
    {
        var itemTemplateId = Guid.NewGuid();
        var itemTemplateRequest = new ItemTemplateRequest();
        var fullyLoadedItemTemplate = new ItemTemplateEntity {Id = itemTemplateId};

        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns(fullyLoadedItemTemplate);

        await _service.EditItemTemplateAsync(new NaheulbookExecutionContext(), itemTemplateId, itemTemplateRequest);

        Received.InOrder(() =>
        {
            _itemTemplateUtil.ApplyChangesFromRequest(fullyLoadedItemTemplate, itemTemplateRequest);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
    }

    [Test]
    public async Task EditItemTemplateAsync_ShouldReplaceSourceUserId_IfSourceIsNotOfficial()
    {
        const int userId = 17;
        var itemTemplateId = Guid.NewGuid();
        var itemTemplateRequest = new ItemTemplateRequest {Source = "private"};
        var fullyLoadedItemTemplate = new ItemTemplateEntity {Id = itemTemplateId};
        var naheulbookExecutionContext = new NaheulbookExecutionContext {UserId = userId};

        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns(fullyLoadedItemTemplate);
        _unitOfWorkFactory.GetUnitOfWork().When(x => x.SaveChangesAsync())
            .Do(_ => fullyLoadedItemTemplate.SourceUserId.Should().Be(userId));

        await _service.EditItemTemplateAsync(naheulbookExecutionContext, itemTemplateId, itemTemplateRequest);

        await _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
    }

    [Test]
    public async Task EditItemTemplateAsync_ShouldSetNullSourceUserId_IfSourceIsOfficial()
    {
        var itemTemplateId = Guid.NewGuid();
        var itemTemplateRequest = new ItemTemplateRequest {Source = "official"};
        var fullyLoadedItemTemplate = new ItemTemplateEntity {Id = itemTemplateId};
        var naheulbookExecutionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns(fullyLoadedItemTemplate);
        _unitOfWorkFactory.GetUnitOfWork().When(x => x.SaveChangesAsync())
            .Do(_ => fullyLoadedItemTemplate.SourceUserId.Should().BeNull());

        await _service.EditItemTemplateAsync(naheulbookExecutionContext, itemTemplateId, itemTemplateRequest);

        await _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
    }

    [Test]
    public async Task EditItemTemplateAsync_CheckPermissionBeforeEditingItem()
    {
        var itemTemplateId = Guid.NewGuid();
        var executionContext = new NaheulbookExecutionContext();
        var fullyLoadedItemTemplate = new ItemTemplateEntity {Id = itemTemplateId};

        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns(fullyLoadedItemTemplate);
        _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, fullyLoadedItemTemplate)
            .Returns(Task.FromException(new TestException()));

        Func<Task<ItemTemplateEntity>> act = () => _service.EditItemTemplateAsync(executionContext, itemTemplateId, new ItemTemplateRequest());

        using (new AssertionScope())
        {
            await act.Should().ThrowAsync<TestException>();
            await _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
        }
    }


    [Test]
    public async Task EditItemTemplateAsync_CheckPermissionBeforeChangingSourceOfAnItemToAdmin()
    {
        var itemTemplateId = Guid.NewGuid();
        var executionContext = new NaheulbookExecutionContext();
        var request = new ItemTemplateRequest {Source = "official"};
        var fullyLoadedItemTemplate = new ItemTemplateEntity {Id = itemTemplateId, Source = "private"};

        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId)
            .Returns(fullyLoadedItemTemplate);
        _authorizationUtil.EnsureAdminAccessAsync(executionContext)
            .Returns(Task.FromException(new TestException()));

        Func<Task<ItemTemplateEntity>> act = () => _service.EditItemTemplateAsync(executionContext, itemTemplateId, request);

        using (new AssertionScope())
        {
            await act.Should().ThrowAsync<TestException>();
            await _unitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
        }
    }

    [Test]
    public async Task SearchItemTemplateAsync_ShouldTryToFindItemsInDatabaseWithMoreAccurateSearchFirstThenWithFuzzyFilter()
    {
        const string filter = "some-filter";
        const string cleanFilter = "SOME-CLEAN-FILTER";
        const string cleanFilterWithoutSeparator = "SOME-CLEAN-FILTER-WITH-NO-SEPARATOR";
        var itemTemplateId1 = Guid.NewGuid();
        var itemTemplateId2 = Guid.NewGuid();
        var itemTemplateId3 = Guid.NewGuid();

        var item1 = new ItemTemplateEntity {Id = itemTemplateId1};
        var item2 = new ItemTemplateEntity {Id = itemTemplateId2};
        var item3 = new ItemTemplateEntity {Id = itemTemplateId3};

        _stringCleanupUtil.RemoveAccents(filter)
            .Returns(cleanFilter);
        _stringCleanupUtil.RemoveSeparators(cleanFilter)
            .Returns(cleanFilterWithoutSeparator);
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetItemByCleanNameWithAllDataAsync(cleanFilter, Arg.Any<int>(), Arg.Any<int?>(), Arg.Any<bool>())
            .Returns(new List<ItemTemplateEntity> {item1});
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetItemByPartialCleanNameWithAllDataAsync(cleanFilter, Arg.Any<int>(), Arg.Any<IEnumerable<Guid>>(), Arg.Any<int?>(), Arg.Any<bool>())
            .Returns(new List<ItemTemplateEntity> {item2});
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(cleanFilterWithoutSeparator, Arg.Any<int>(), Arg.Any<IEnumerable<Guid>>(), Arg.Any<int?>(), Arg.Any<bool>())
            .Returns(new List<ItemTemplateEntity> {item3});

        var actualItemTemplates = await _service.SearchItemTemplateAsync(filter, 40, null);

        actualItemTemplates.Should().BeEquivalentTo(new [] {item1, item2, item3});
    }

    [Test]
    public async Task SearchItemTemplateAsync_ShouldLimitResultToGivenMaximumCount()
    {
        _stringCleanupUtil.RemoveAccents(Arg.Any<string>())
            .Returns("some-clean-filter");
        _stringCleanupUtil.RemoveSeparators(Arg.Any<string>())
            .Returns("some-clean-name-with-no-separator");
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetItemByCleanNameWithAllDataAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int?>(), Arg.Any<bool>())
            .Returns(new List<ItemTemplateEntity> {new ItemTemplateEntity(), new ItemTemplateEntity()});
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetItemByPartialCleanNameWithAllDataAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IEnumerable<Guid>>(), Arg.Any<int?>(), Arg.Any<bool>())
            .Returns(new List<ItemTemplateEntity> {new ItemTemplateEntity(), new ItemTemplateEntity(), new ItemTemplateEntity()});
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IEnumerable<Guid>>(), Arg.Any<int?>(), Arg.Any<bool>())
            .Returns(new List<ItemTemplateEntity> {new ItemTemplateEntity()});

        await _service.SearchItemTemplateAsync("some-filter", 10, null);

        await _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.Received(1).GetItemByCleanNameWithAllDataAsync(Arg.Any<string>(), 10, Arg.Any<int?>(), Arg.Any<bool>());
        await _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.Received(1).GetItemByPartialCleanNameWithAllDataAsync(Arg.Any<string>(), 8, Arg.Any<IEnumerable<Guid>>(), Arg.Any<int?>(), Arg.Any<bool>());
        await _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.Received(1).GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(Arg.Any<string>(), 5, Arg.Any<IEnumerable<Guid>>(), Arg.Any<int?>(), Arg.Any<bool>());
    }
}