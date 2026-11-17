using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class ItemTemplateSubCategoriesControllerTests
{
    private IItemTemplateSubCategoryService _itemTemplateSubCategoryService;
    private IItemTemplateMapper _itemTemplateMapper;
    private ItemTemplateSubCategoriesController _itemTemplateSubCategoriesController;
    private NaheulbookExecutionContext _executionContext;

    [SetUp]
    public void SetUp()
    {
        _itemTemplateSubCategoryService = Substitute.For<IItemTemplateSubCategoryService>();
        _itemTemplateMapper = Substitute.For<IItemTemplateMapper>();
        _itemTemplateSubCategoriesController = new ItemTemplateSubCategoriesController(_itemTemplateSubCategoryService, _itemTemplateMapper);
        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task PostCreateItemTemplateSubCategoryAsync_CallItemSubCategoryService()
    {
        var createItemTemplateSubCategoryRequest = new CreateItemTemplateSubCategoryRequest {Name = string.Empty};
        var itemTemplateSubCategory = new ItemTemplateSubCategoryEntity();
        var itemTemplateSubCategoryResponse = new ItemTemplateSubCategoryResponse();

        _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(_executionContext, createItemTemplateSubCategoryRequest)
            .Returns(itemTemplateSubCategory);
        _itemTemplateMapper.ToResponse(itemTemplateSubCategory)
            .Returns(itemTemplateSubCategoryResponse);

        var result = await _itemTemplateSubCategoriesController.PostCreateItemTemplateSubCategoryAsync(_executionContext, createItemTemplateSubCategoryRequest);

        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(itemTemplateSubCategoryResponse);
    }

    [Test]
    public async Task PostCreateItemTemplateSubCategoryAsync_WhenCatchForbiddenAccessException_Return403()
    {
        var createItemTemplateSubCategoryRequest = new CreateItemTemplateSubCategoryRequest {Name = string.Empty};

        _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateItemTemplateSubCategoryRequest>())
            .Returns(Task.FromException<ItemTemplateSubCategoryEntity>(new ForbiddenAccessException()));

        Func<Task> act = () => _itemTemplateSubCategoriesController.PostCreateItemTemplateSubCategoryAsync(_executionContext, createItemTemplateSubCategoryRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}