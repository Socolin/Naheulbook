using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
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
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class ItemTemplatesControllerTests
    {
        private IItemTemplateService _itemTemplateService;
        private IMapper _mapper;
        private ItemTemplatesController _itemTemplatesController;
        private NaheulbookExecutionContext _executionContext;

        [SetUp]
        public void SetUp()
        {
            _itemTemplateService = Substitute.For<IItemTemplateService>();
            _mapper = Substitute.For<IMapper>();
            _itemTemplatesController = new ItemTemplatesController(_itemTemplateService, _mapper);
            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task GetItemTemplateAsync_RetrieveItemInfroFromItemService_AndMapItIntoResponse()
        {
            const int itemTemplateId = 42;
            var itemTemplate = new ItemTemplate();
            var itemTemplateResponse = new ItemTemplateResponse();

            _itemTemplateService.GetItemTemplateAsync(itemTemplateId)
                .Returns(itemTemplate);
            _mapper.Map<ItemTemplateResponse>(itemTemplate)
                .Returns(itemTemplateResponse);

            var result = await _itemTemplatesController.GetItemTemplateAsync(itemTemplateId);

            result.Value.Should().Be(itemTemplateResponse);
        }

        [Test]
        public void GetItemTemplateAsync_Return404_WhenItemTemplateNotFoundIsThrow()
        {
            _itemTemplateService.GetItemTemplateAsync(Arg.Any<int>())
                .Throws(new ItemTemplateNotFoundException(42));

            Func<Task> act = () => _itemTemplatesController.GetItemTemplateAsync(42);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        [Test]
        public async Task PutItemTemplateAsync_CallServiceToEditItem_AndMapEditedItemIntoResponse()
        {
            const int itemTemplateId = 42;
            var itemTemplate = new ItemTemplate();
            var itemTemplateRequest = new ItemTemplateRequest();
            var itemTemplateResponse = new ItemTemplateResponse();

            _itemTemplateService.EditItemTemplateAsync(_executionContext, 42, itemTemplateRequest)
                .Returns(itemTemplate);
            _mapper.Map<ItemTemplateResponse>(itemTemplate)
                .Returns(itemTemplateResponse);

            var result = await _itemTemplatesController.PutItemTemplateAsync(_executionContext, itemTemplateId, itemTemplateRequest);

            result.Value.Should().Be(itemTemplateResponse);
        }

        [Test]
        public void PutItemTemplateAsync_Return404_WhenItemTemplateNotFoundIsThrow()
        {
            _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<ItemTemplateRequest>())
                .Throws(new ItemTemplateNotFoundException(42));

            Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, 42, new ItemTemplateRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public void PutItemTemplateAsync_Return403_WhenNotAllowed()
        {
            _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<ItemTemplateRequest>())
                .Throws(new ForbiddenAccessException());

            Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, 42, new ItemTemplateRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task PostCreateItemTemplate_CallItemService()
        {
            var itemTemplateRequest = new ItemTemplateRequest();
            var itemTemplate = new ItemTemplate();
            var itemTemplateResponse = new ItemTemplateResponse();

            _itemTemplateService.CreateItemTemplateAsync(_executionContext, itemTemplateRequest)
                .Returns(itemTemplate);
            _mapper.Map<ItemTemplateResponse>(itemTemplate)
                .Returns(itemTemplateResponse);

            var result = await _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, itemTemplateRequest);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(itemTemplateResponse);
        }

        [Test]
        public void PostCreateItemTemplate_WhenCatchForbiddenAccessException_Return403()
        {
            _itemTemplateService.CreateItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<ItemTemplateRequest>())
                .Returns(Task.FromException<ItemTemplate>(new ForbiddenAccessException()));

            Func<Task<JsonResult>> act = () => _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, new ItemTemplateRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}