using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ItemTemplateCategoriesControllerTests
    {
        private IItemTemplateCategoryService _itemTemplateCategoryService;
        private IMapper _mapper;
        private ItemTemplateCategoriesController _itemTemplateCategoriesController;
        private NaheulbookExecutionContext _executionContext;

        [SetUp]
        public void SetUp()
        {
            _itemTemplateCategoryService = Substitute.For<IItemTemplateCategoryService>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplateCategoriesController = new ItemTemplateCategoriesController(_itemTemplateCategoryService, _mapper);
            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task PostCreateCategory_CallItemCategoryService()
        {
            var createItemTemplateCategoryRequest = new CreateItemTemplateCategoryRequest();
            var itemTemplateCategory = new ItemTemplateCategory();
            var itemTemplateCategoryResponse = new ItemTemplateCategoryResponse();

            _itemTemplateCategoryService.CreateItemTemplateCategoryAsync(_executionContext, createItemTemplateCategoryRequest)
                .Returns(itemTemplateCategory);
            _mapper.Map<ItemTemplateCategoryResponse>(itemTemplateCategory)
                .Returns(itemTemplateCategoryResponse);

            var result = await _itemTemplateCategoriesController.PostCreateCategoryAsync(_executionContext, createItemTemplateCategoryRequest);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(itemTemplateCategoryResponse);
        }

        [Test]
        public void PostCreateCategory_WhenCatchForbiddenAccessException_Return403()
        {
            _itemTemplateCategoryService.CreateItemTemplateCategoryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateItemTemplateCategoryRequest>())
                .Returns(Task.FromException<ItemTemplateCategory>(new ForbiddenAccessException()));

            Func<Task<JsonResult>> act = () => _itemTemplateCategoriesController.PostCreateCategoryAsync(_executionContext, new CreateItemTemplateCategoryRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }
    }
}