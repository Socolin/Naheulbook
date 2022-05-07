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
            var itemTemplateId = Guid.NewGuid();
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
        public async Task GetItemTemplateAsync_Return404_WhenItemTemplateNotFoundIsThrow()
        {
            var itemTemplateId = Guid.NewGuid();
            _itemTemplateService.GetItemTemplateAsync(Arg.Any<Guid>())
                .Throws(new ItemTemplateNotFoundException(itemTemplateId));

            Func<Task> act = () => _itemTemplatesController.GetItemTemplateAsync(itemTemplateId);

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
        [Test]
        public async Task PutItemTemplateAsync_CallServiceToEditItem_AndMapEditedItemIntoResponse()
        {
            var itemTemplateId = Guid.NewGuid();
            var itemTemplate = new ItemTemplate();
            var itemTemplateRequest = new ItemTemplateRequest();
            var itemTemplateResponse = new ItemTemplateResponse();

            _itemTemplateService.EditItemTemplateAsync(_executionContext, itemTemplateId, itemTemplateRequest)
                .Returns(itemTemplate);
            _mapper.Map<ItemTemplateResponse>(itemTemplate)
                .Returns(itemTemplateResponse);

            var result = await _itemTemplatesController.PutItemTemplateAsync(_executionContext, itemTemplateId, itemTemplateRequest);

            result.Value.Should().Be(itemTemplateResponse);
        }

        [Test]
        public async Task PutItemTemplateAsync_Return404_WhenItemTemplateNotFoundIsThrow()
        {
            var itemTemplateId = Guid.NewGuid();
            _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<Guid>(), Arg.Any<ItemTemplateRequest>())
                .Throws(new ItemTemplateNotFoundException(itemTemplateId));

            Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, itemTemplateId, new ItemTemplateRequest());

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Test]
        public async Task PutItemTemplateAsync_Return403_WhenNotAllowed()
        {
            _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<Guid>(), Arg.Any<ItemTemplateRequest>())
                .Throws(new ForbiddenAccessException());

            Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, Guid.NewGuid(), new ItemTemplateRequest());

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
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
        public async Task PostCreateItemTemplate_WhenCatchForbiddenAccessException_Return403()
        {
            _itemTemplateService.CreateItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<ItemTemplateRequest>())
                .Returns(Task.FromException<ItemTemplate>(new ForbiddenAccessException()));

            Func<Task<JsonResult>> act = () => _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, new ItemTemplateRequest());

            (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }
    }
}