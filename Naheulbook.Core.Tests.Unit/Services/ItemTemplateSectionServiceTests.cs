using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestUtils.AutoFillTestObjects;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemTemplateSectionServiceTests
    {
        private IItemTemplateSectionRepository _itemTemplateSectionRepository;
        private IUnitOfWork _unitOfWork;
        private IAuthorizationUtil _authorizationUtil;
        private ItemTemplateSectionService _itemTemplateSectionService;

        [SetUp]
        public void SetUp()
        {
            var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
            _itemTemplateSectionRepository = Substitute.For<IItemTemplateSectionRepository>();
            _unitOfWork.ItemTemplateSections.Returns(_itemTemplateSectionRepository);
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _itemTemplateSectionService = new ItemTemplateSectionService(unitOfWorkFactory, _authorizationUtil);
        }

        [Test]
        public async Task CreateItemTemplateSection_AddANewItemTemplateSectionInDatabase()
        {
            var expectedItemTemplateSection = CreateItemTemplateSection();
            var createItemTemplateSectionRequest = AutoFill<CreateItemTemplateSectionRequest>.One(settings: new AutoFillSettings {MaxDepth = 0});

            var itemTemplateSection = await _itemTemplateSectionService.CreateItemTemplateSectionAsync(new NaheulbookExecutionContext(), createItemTemplateSectionRequest);

            Received.InOrder(() =>
            {
                _itemTemplateSectionRepository.Add(itemTemplateSection);
                _unitOfWork.CompleteAsync();
            });
            itemTemplateSection.Should().BeEquivalentTo(expectedItemTemplateSection);
        }

        [Test]
        public async Task CreateItemTemplateSection_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();

            await _itemTemplateSectionService.CreateItemTemplateSectionAsync(executionContext, new CreateItemTemplateSectionRequest());

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWork.CompleteAsync();
            });
        }

        private ItemTemplateSection CreateItemTemplateSection()
        {
            return new ItemTemplateSection
            {
                Name = "some-name",
                Special = "some-specials0,some-specials1,some-specials2",
                Note = "some-note",
                Categories = new List<ItemTemplateCategory>()
            };
        }
    }
}