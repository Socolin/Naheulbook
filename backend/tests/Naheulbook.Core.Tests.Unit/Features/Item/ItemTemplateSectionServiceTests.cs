using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Item;

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
        var createItemTemplateSectionRequest = new CreateItemTemplateSectionRequest
        {
            Name = "some-name",
            Icon = "some-icon",
            Note = "some-note",
            Specials =
            [
                "some-specials0",
                "some-specials1",
                "some-specials2",
            ],
        };

        var itemTemplateSection = await _service.CreateItemTemplateSectionAsync(new NaheulbookExecutionContext(), createItemTemplateSectionRequest);

        var itemTemplateSectionRepository = _unitOfWorkFactory.GetUnitOfWork().ItemTemplateSections;
        Received.InOrder(() =>
        {
            itemTemplateSectionRepository.Add(itemTemplateSection);
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
        itemTemplateSection.Should().BeEquivalentTo(expectedItemTemplateSection);
    }

    [Test]
    public async Task CreateItemTemplateSection_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
    {
        var request = new CreateItemTemplateSectionRequest {Name = string.Empty, Icon = string.Empty, Specials = []};
        var executionContext = new NaheulbookExecutionContext();

        await _service.CreateItemTemplateSectionAsync(executionContext, request);

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
        var allSectionItemTemplates = new List<ItemTemplateEntity> {new(), new()};
        var filteredItemTemplate = new List<ItemTemplateEntity> {new()};

        _unitOfWorkFactory.GetUnitOfWork().ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(sectionId)
            .Returns(allSectionItemTemplates);
        _itemTemplateUtil.FilterItemTemplatesBySource(allSectionItemTemplates, userId, true)
            .Returns(filteredItemTemplate);

        var actualItemTemplates = await _service.GetItemTemplatesBySectionAsync(sectionId, userId);

        actualItemTemplates.Should().BeEquivalentTo(filteredItemTemplate);
    }

    private ItemTemplateSectionEntity CreateItemTemplateSection()
    {
        return new ItemTemplateSectionEntity
        {
            Name = "some-name",
            Special = "some-specials0,some-specials1,some-specials2",
            Note = "some-note",
            Icon = "some-icon",
            SubCategories = new List<ItemTemplateSubCategoryEntity>(),
        };
    }
}