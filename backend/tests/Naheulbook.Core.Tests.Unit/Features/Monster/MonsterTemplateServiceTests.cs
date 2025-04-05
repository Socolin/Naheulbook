using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Monster;

public class MonsterTemplateServiceTests
{
    private FakeUnitOfWorkFactory _unitOfWorkFactory;
    private IAuthorizationUtil _authorizationUtil;
    private MonsterTemplateService _service;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkFactory = new FakeUnitOfWorkFactory();
        _authorizationUtil = Substitute.For<IAuthorizationUtil>();

        _service = new MonsterTemplateService(_unitOfWorkFactory, _authorizationUtil);
    }

    [Test]
    public async Task CreateMonsterTemplate_InsertNewEntityInDatabase()
    {
        const int subCategoryId = 10;
        const int locationId = 12;
        var itemTemplateId = Guid.NewGuid();

        var executionContext = new NaheulbookExecutionContext();
        var monsterSubCategory = CreateMonsterSubCategory(subCategoryId);
        var request = CreateRequest(subCategoryId, itemTemplateId, locationId);
        var itemTemplate = new ItemTemplateEntity {Id = itemTemplateId};
        var expectedMonsterTemplate = CreateMonsterTemplate(monsterSubCategory, itemTemplate);

        _unitOfWorkFactory.GetUnitOfWork().MonsterSubCategories.GetAsync(subCategoryId)
            .Returns(monsterSubCategory);
        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetByIdsAsync(Arg.Is<IEnumerable<Guid>>(x => x.SequenceEqual(new[] {itemTemplateId})))
            .Returns([itemTemplate]);

        var monsterTemplate = await _service.CreateMonsterTemplateAsync(executionContext, request);

        var monsterTemplateRepository = _unitOfWorkFactory.GetUnitOfWork().MonsterTemplates;
        Received.InOrder(() =>
        {
            monsterTemplateRepository.Add(monsterTemplate);
            _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
        });
        monsterTemplate.Should().BeEquivalentTo(expectedMonsterTemplate);
    }

    [Test]
    public async Task CreateMonsterTemplate_EnsureAdminAccess()
    {
        var request = new MonsterTemplateRequest {Name = string.Empty, Data = new JObject()};
        var executionContext = new NaheulbookExecutionContext();

        _authorizationUtil.EnsureAdminAccessAsync(executionContext)
            .Throws(new TestException());

        Func<Task> act = () => _service.CreateMonsterTemplateAsync(executionContext, request);

        await act.Should().ThrowAsync<TestException>();
    }

    [Test]
    public async Task CreateMonsterTemplate_WhenRequestedSubCategoryIdDoesNotExists_ThrowMonsterSubCategoryNotFoundException()
    {
        var request = new MonsterTemplateRequest
        {
            Name = string.Empty,
            Data = new JObject(),
            SubCategoryId = 42,
        };
        var executionContext = new NaheulbookExecutionContext();

        _unitOfWorkFactory.GetUnitOfWork().MonsterSubCategories.GetAsync(42)
            .Returns((MonsterSubCategoryEntity) null);

        Func<Task> act = () => _service.CreateMonsterTemplateAsync(executionContext, request);

        await act.Should().ThrowAsync<MonsterSubCategoryNotFoundException>();
    }

    private static MonsterSubCategoryEntity CreateMonsterSubCategory(int subCategoryId)
    {
        return new MonsterSubCategoryEntity
        {
            Id = subCategoryId,
        };
    }

    private static MonsterTemplateRequest CreateRequest(int subCategoryId, Guid itemTemplateId, int locationId)
    {
        return new MonsterTemplateRequest
        {
            SubCategoryId = subCategoryId,
            Data = JObject.FromObject(new {key = "value"}),
            LocationIds = new List<int> {locationId},
            Name = "some-monster-name",
            Inventory = new List<MonsterTemplateInventoryElementRequest>
            {
                new()
                {
                    Chance = 0.5f,
                    MinCount = 1,
                    MaxCount = 2,
                    ItemTemplateId = itemTemplateId,
                },
            },
        };
    }

    private static MonsterTemplateEntity CreateMonsterTemplate(MonsterSubCategoryEntity monsterSubCategory, ItemTemplateEntity itemTemplate)
    {
        return new MonsterTemplateEntity()
        {
            SubCategory = monsterSubCategory,
            Data = @"{""key"":""value""}",
            Items = new List<MonsterTemplateInventoryElementEntity>()
            {
                new()
                {
                    Chance = 0.5f,
                    MinCount = 1,
                    MaxCount = 2,
                    ItemTemplate = itemTemplate,
                },
            },
            Name = "some-monster-name",
        };
    }
}