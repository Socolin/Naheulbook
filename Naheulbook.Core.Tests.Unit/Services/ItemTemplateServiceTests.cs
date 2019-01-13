using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.Exceptions;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemTemplateServiceTests
    {
        private IItemTemplateRepository _itemTemplateRepository;
        private IUnitOfWork _unitOfWork;
        private IAuthorizationUtil _authorizationUtil;
        private ItemTemplateService _itemTemplateService;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
            _itemTemplateRepository = Substitute.For<IItemTemplateRepository>();
            _unitOfWork.ItemTemplates.Returns(_itemTemplateRepository);
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplateService = new ItemTemplateService(unitOfWorkFactory, _authorizationUtil, _mapper);
        }

        [Test]
        public async Task CreateItemTemplate_AddANewItemTemplateInDatabase()
        {
            var expectedItemTemplate = new ItemTemplate();
            var createItemTemplateRequest = new CreateItemTemplateRequest();

            _mapper.Map<ItemTemplate>(createItemTemplateRequest)
                .Returns(expectedItemTemplate);

            var itemTemplate = await _itemTemplateService.CreateItemTemplateAsync(new NaheulbookExecutionContext(), createItemTemplateRequest);

            Received.InOrder(() =>
            {
                _itemTemplateRepository.Add(itemTemplate);
                _unitOfWork.CompleteAsync();
            });
            itemTemplate.Should().BeEquivalentTo(expectedItemTemplate);
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
            await _unitOfWork.DidNotReceive().CompleteAsync();
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