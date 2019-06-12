using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.TestUtils;
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

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplateService = new ItemTemplateService(_unitOfWorkFactory, _authorizationUtil, _mapper);
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
            var createItemTemplateRequest = new CreateItemTemplateRequest();

            _mapper.Map<ItemTemplate>(createItemTemplateRequest)
                .Returns(newItemTemplateEntity);
            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(newItemTemplateEntity.Id)
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
            var createItemTemplateRequest = new CreateItemTemplateRequest {Source = "official"};

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
            var expectedItemTemplate = new ItemTemplate();
            var createItemTemplateRequest = new CreateItemTemplateRequest {Source = "non-official"};

            _mapper.Map<ItemTemplate>(createItemTemplateRequest)
                .Returns(expectedItemTemplate);

            var itemTemplate = await _itemTemplateService.CreateItemTemplateAsync(executionContext, createItemTemplateRequest);

            itemTemplate.SourceUserId.Should().Be(42);
        }
    }
}