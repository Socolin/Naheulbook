using FluentAssertions;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Item;

public class ItemTemplateSubCategoryServiceTests
{
    private IItemTemplateSubCategoryRepository _itemTemplateSubCategoryRepository;
    private IUnitOfWork _unitOfWork;
    private IAuthorizationUtil _authorizationUtil;
    private ItemTemplateSubCategoryService _itemTemplateSubCategoryService;

    [SetUp]
    public void SetUp()
    {
        var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
        _itemTemplateSubCategoryRepository = Substitute.For<IItemTemplateSubCategoryRepository>();
        _unitOfWork.ItemTemplateSubCategories.Returns(_itemTemplateSubCategoryRepository);
        _authorizationUtil = Substitute.For<IAuthorizationUtil>();
        _itemTemplateSubCategoryService = new ItemTemplateSubCategoryService(unitOfWorkFactory, _authorizationUtil);
    }

    [Test]
    public async Task CreateItemTemplateSubCategory_AddANewItemTemplateSubCategoryInDatabase()
    {
        var expectedItemTemplateSubCategory = CreateItemTemplateSubCategoryAsync();
        var request = new CreateItemTemplateSubCategoryRequest
        {
            Name = "some-name",
            TechName = "some-tech-name",
            Description = "some-description",
            Note = "some-note",
            SectionId = 1,
        };

        var itemTemplateSubCategory = await _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(new NaheulbookExecutionContext(), request);

        Received.InOrder(() =>
        {
            _itemTemplateSubCategoryRepository.Add(itemTemplateSubCategory);
            _unitOfWork.SaveChangesAsync();
        });
        itemTemplateSubCategory.Should().BeEquivalentTo(expectedItemTemplateSubCategory);
    }

    [Test]
    public async Task CreateItemTemplateSubCategoryAsync_EnsureThatUserIsAnAdmin_BeforeAddingInDatabase()
    {
        var request = new CreateItemTemplateSubCategoryRequest {Name = string.Empty};
        var executionContext = new NaheulbookExecutionContext();

        await _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(executionContext, request);

        Received.InOrder(() =>
        {
            _authorizationUtil.EnsureAdminAccessAsync(executionContext);
            _unitOfWork.SaveChangesAsync();
        });
    }

    private ItemTemplateSubCategoryEntity CreateItemTemplateSubCategoryAsync()
    {
        return new ItemTemplateSubCategoryEntity
        {
            Name = "some-name",
            TechName = "some-tech-name",
            Description = "some-description",
            Note = "some-note",
            SectionId = 1,
        };
    }
}