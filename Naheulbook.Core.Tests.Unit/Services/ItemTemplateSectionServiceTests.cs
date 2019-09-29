using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;
using Socolin.TestUtils.AutoFillTestObjects;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class ItemTemplateSectionServiceTests
    {
        private FakeUnitOfWorkFactory _unitOfWorkFactory;
        private IAuthorizationUtil _authorizationUtil;
        private IItemTemplateUtil _itemTemplateUtil;

        private ItemTemplateSectionService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkFactory = new FakeUnitOfWorkFactory();
            _authorizationUtil = Substitute.For<IAuthorizationUtil>();
            _itemTemplateUtil = Substitute.For<IItemTemplateUtil>();

            _service = new ItemTemplateSectionService(
                _unitOfWorkFactory,
                _authorizationUtil,
                _itemTemplateUtil
            );
        }

        [Test]
        public async Task CreateItemTemplateSection_AddANewItemTemplateSectionInDatabase()
        {
            var expectedItemTemplateSection = CreateItemTemplateSection();
            var createItemTemplateSectionRequest = AutoFill<CreateItemTemplateSectionRequest>.One(settings: new AutoFillSettings {MaxDepth = 0});

            var itemTemplateSection = await _service.CreateItemTemplateSectionAsync(new NaheulbookExecutionContext(), createItemTemplateSectionRequest);

            Received.InOrder(() =>
            {
                _unitOfWorkFactory.GetUnitOfWork().ItemTemplateSections.Add(itemTemplateSection);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            });
            itemTemplateSection.Should().BeEquivalentTo(expectedItemTemplateSection);
        }

        [Test]
        public async Task CreateItemTemplateSection_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
        {
            var executionContext = new NaheulbookExecutionContext();

            await _service.CreateItemTemplateSectionAsync(executionContext, new CreateItemTemplateSectionRequest());

            Received.InOrder(() =>
            {
                _authorizationUtil.EnsureAdminAccessAsync(executionContext);
                _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
            });
        }

        [Test]
        public async Task GetItemTemplatesBySectionAsync_ShouldFilterItemTemplatesBasedOnSource()
        {
            const int sectionId = 8;
            const int userId = 12;
            var allSectionItemTemplates = new List<ItemTemplate> {new ItemTemplate(), new ItemTemplate()};
            var filteredItemTemplate = new List<ItemTemplate> {new ItemTemplate()};

            _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(sectionId)
                .Returns(allSectionItemTemplates);
            _itemTemplateUtil.FilterItemTemplatesBySource(allSectionItemTemplates, userId, true)
                .Returns(filteredItemTemplate);

            var actualItemTemplates = await _service.GetItemTemplatesBySectionAsync(sectionId, userId);

            actualItemTemplates.Should().BeEquivalentTo(filteredItemTemplate);
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