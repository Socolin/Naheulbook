using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class ItemTemplateSubCategoriesControllerTests
    {
        private IItemTemplateSubCategoryService _itemTemplateSubCategoryService;
        private IMapper _mapper;
        private ItemTemplateSubCategoriesController _itemTemplateSubCategoriesController;
        private NaheulbookExecutionContext _executionContext;

        [SetUp]
        public void SetUp()
        {
            _itemTemplateSubCategoryService = Substitute.For<IItemTemplateSubCategoryService>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplateSubCategoriesController = new ItemTemplateSubCategoriesController(_itemTemplateSubCategoryService, _mapper);
            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task PostCreateItemTemplateSubCategoryAsync_CallItemSubCategoryService()
        {
            var createItemTemplateSubCategoryRequest = new CreateItemTemplateSubCategoryRequest();
            var itemTemplateSubCategory = new ItemTemplateSubCategoryEntity();
            var itemTemplateSubCategoryResponse = new ItemTemplateSubCategoryResponse();

            _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(_executionContext, createItemTemplateSubCategoryRequest)
                .Returns(itemTemplateSubCategory);
            _mapper.Map<ItemTemplateSubCategoryResponse>(itemTemplateSubCategory)
                .Returns(itemTemplateSubCategoryResponse);

            var result = await _itemTemplateSubCategoriesController.PostCreateItemTemplateSubCategoryAsync(_executionContext, createItemTemplateSubCategoryRequest);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(itemTemplateSubCategoryResponse);
        }

        [Test]
        public async Task PostCreateItemTemplateSubCategoryAsync_WhenCatchForbiddenAccessException_Return403()
        {
            _itemTemplateSubCategoryService.CreateItemTemplateSubCategoryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateItemTemplateSubCategoryRequest>())
                .Returns(Task.FromException<ItemTemplateSubCategoryEntity>(new ForbiddenAccessException()));

            Func<Task> act = () => _itemTemplateSubCategoriesController.PostCreateItemTemplateSubCategoryAsync(_executionContext, new CreateItemTemplateSubCategoryRequest());

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }
    }
}