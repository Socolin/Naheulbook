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
            const int categoryId = 10;
            const int itemTemplateId = 11;
            const int locationId = 12;

            var executionContext = new NaheulbookExecutionContext();
            var monsterCategory = CreateMonsterCategory(categoryId);
            var request = CreateRequest(categoryId, itemTemplateId, locationId);
            var location = new Location();
            var itemTemplate = new ItemTemplate {Id = itemTemplateId};
            var expectedMonsterTemplate = CreateMonsterTemplate(monsterCategory, itemTemplate, location);

            _unitOfWorkFactory.GetUnitOfWork().MonsterCategories.GetAsync(categoryId)
                .Returns(monsterCategory);
            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetByIdsAsync(Arg.Is<IEnumerable<int>>(x => x.SequenceEqual(new[] {itemTemplateId})))
                .Returns(new List<ItemTemplate> {itemTemplate});
            _unitOfWorkFactory.GetUnitOfWork().Locations.GetByIdsAsync(Arg.Is<IEnumerable<int>>(x => x.SequenceEqual(new[] {locationId})))
                .Returns(new List<Location> {location});

            var monsterTemplate = await _service.CreateMonsterTemplate(executionContext, request);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().MonsterTemplates.Add(monsterTemplate);
                _unitOfWorkFactory.GetUnitOfWork().Received(1).CompleteAsync();
            });
            monsterTemplate.Should().BeEquivalentTo(expectedMonsterTemplate);
        }

        [Test]
        public void CreateMonsterTemplate_EnsureAdminAccess()
        {
            var request = new CreateMonsterTemplateRequest();
            var executionContext = new NaheulbookExecutionContext();

            _authorizationUtil.EnsureAdminAccessAsync(executionContext)
                .Throws(new TestException());

            Func<Task> act = () => _service.CreateMonsterTemplate(executionContext, request);

            act.Should().Throw<TestException>();
        }

        [Test]
        public void CreateMonsterTemplate_WhenRequestedCategoryIdDoesNotExists_ThrowMonsterCategoryNotFoundException()
        {
            var request = new CreateMonsterTemplateRequest
            {
                CategoryId = 42
            };
            var executionContext = new NaheulbookExecutionContext();

            _unitOfWorkFactory.GetUnitOfWork().MonsterCategories.GetAsync(42)
                .Returns((MonsterCategory) null);

            Func<Task> act = () => _service.CreateMonsterTemplate(executionContext, request);

            act.Should().Throw<MonsterCategoryNotFoundException>();
        }

        private static MonsterCategory CreateMonsterCategory(int categoryId)
        {
            return new MonsterCategory
            {
                Id = categoryId
            };
        }

        private static CreateMonsterTemplateRequest CreateRequest(int categoryId, int itemTemplateId, int locationId)
        {
            return new CreateMonsterTemplateRequest
            {
                CategoryId = categoryId,
                Monster = new MonsterTemplateRequest
                {
                    Data = JObject.FromObject(new {key = "value"}),
                    LocationIds = new List<int> {locationId},
                    Name = "some-monster-name",
                    SimpleInventory = new List<MonsterSimpleInventoryRequest>
                    {
                        new MonsterSimpleInventoryRequest
                        {
                            Chance = 0.5f,
                            MinCount = 1,
                            MaxCount = 2,
                            ItemTemplate = new IdRequest
                            {
                                Id = itemTemplateId
                            },
                        }
                    }
                }
            };
        }

        private static MonsterTemplate CreateMonsterTemplate(MonsterCategory monsterCategory, ItemTemplate itemTemplate, Location location)
        {
            return new MonsterTemplate()
            {
                Category = monsterCategory,
                Data = @"{""key"":""value""}",
                Items = new List<MonsterTemplateSimpleInventory>()
                {
                    new MonsterTemplateSimpleInventory
                    {
                        Chance = 0.5f,
                        MinCount = 1,
                        MaxCount = 2,
                        ItemTemplate = itemTemplate,
                    }
                },
                Locations = new List<MonsterLocation>
                {
                    new MonsterLocation
                    {
                        Location = location
                    }
                },
                Name = "some-monster-name"
            };
        }
    }
}