using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
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
            const int itemTemplateId = 11;
            const int locationId = 12;

            var executionContext = new NaheulbookExecutionContext();
            var monsterSubCategory = CreateMonsterSubCategory(subCategoryId);
            var request = CreateRequest(subCategoryId, itemTemplateId, locationId);
            var itemTemplate = new ItemTemplate {Id = itemTemplateId};
            var expectedMonsterTemplate = CreateMonsterTemplate(monsterSubCategory, itemTemplate);

            _unitOfWorkFactory.GetUnitOfWork().MonsterSubCategories.GetAsync(subCategoryId)
                .Returns(monsterSubCategory);
            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetByIdsAsync(Arg.Is<IEnumerable<int>>(x => x.SequenceEqual(new[] {itemTemplateId})))
                .Returns(new List<ItemTemplate> {itemTemplate});

            var monsterTemplate = await _service.CreateMonsterTemplateAsync(executionContext, request);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().MonsterTemplates.Add(monsterTemplate);
                _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
            });
            monsterTemplate.Should().BeEquivalentTo(expectedMonsterTemplate);
        }

        [Test]
        public void CreateMonsterTemplate_EnsureAdminAccess()
        {
            var request = new MonsterTemplateRequest();
            var executionContext = new NaheulbookExecutionContext();

            _authorizationUtil.EnsureAdminAccessAsync(executionContext)
                .Throws(new TestException());

            Func<Task> act = () => _service.CreateMonsterTemplateAsync(executionContext, request);

            act.Should().Throw<TestException>();
        }

        [Test]
        public void CreateMonsterTemplate_WhenRequestedSubCategoryIdDoesNotExists_ThrowMonsterSubCategoryNotFoundException()
        {
            var request = new MonsterTemplateRequest
            {
                SubCategoryId = 42
            };
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().MonsterSubCategories.GetAsync(42)
                .Returns((MonsterSubCategory) null);

            Func<Task> act = () => _service.CreateMonsterTemplateAsync(executionContext, request);

            act.Should().Throw<MonsterSubCategoryNotFoundException>();
        }

        private static MonsterSubCategory CreateMonsterSubCategory(int subCategoryId)
        {
            return new MonsterSubCategory
            {
                Id = subCategoryId
            };
        }

        private static MonsterTemplateRequest CreateRequest(int subCategoryId, int itemTemplateId, int locationId)
        {
            return new MonsterTemplateRequest
            {
                SubCategoryId = subCategoryId,
                Data = JObject.FromObject(new {key = "value"}),
                LocationIds = new List<int> {locationId},
                Name = "some-monster-name",
                Inventory = new List<MonsterTemplateInventoryElementRequest>
                {
                    new MonsterTemplateInventoryElementRequest
                    {
                        Chance = 0.5f,
                        MinCount = 1,
                        MaxCount = 2,
                        ItemTemplateId = itemTemplateId
                    }
                }
            };
        }

        private static MonsterTemplate CreateMonsterTemplate(MonsterSubCategory monsterSubCategory, ItemTemplate itemTemplate)
        {
            return new MonsterTemplate()
            {
                SubCategory = monsterSubCategory,
                Data = @"{""key"":""value""}",
                Items = new List<MonsterTemplateInventoryElement>()
                {
                    new MonsterTemplateInventoryElement
                    {
                        Chance = 0.5f,
                        MinCount = 1,
                        MaxCount = 2,
                        ItemTemplate = itemTemplate,
                    }
                },
                Name = "some-monster-name"
            };
        }
    }
}