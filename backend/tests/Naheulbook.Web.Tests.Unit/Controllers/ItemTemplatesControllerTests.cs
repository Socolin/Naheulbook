using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers;

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
        var itemTemplate = new ItemTemplateEntity();
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
        var itemTemplate = new ItemTemplateEntity();
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};
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
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};

        _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<Guid>(), Arg.Any<ItemTemplateRequest>())
            .Throws(new ItemTemplateNotFoundException(itemTemplateId));

        Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, itemTemplateId, itemTemplateRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task PutItemTemplateAsync_Return403_WhenNotAllowed()
    {
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};

        _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<Guid>(), Arg.Any<ItemTemplateRequest>())
            .Throws(new ForbiddenAccessException());

        Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, Guid.NewGuid(), itemTemplateRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Test]
    public async Task PostCreateItemTemplate_CallItemService()
    {
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};
        var itemTemplate = new ItemTemplateEntity();
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
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};

        _itemTemplateService.CreateItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<ItemTemplateRequest>())
            .Returns(Task.FromException<ItemTemplateEntity>(new ForbiddenAccessException()));

        Func<Task<JsonResult>> act = () => _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, itemTemplateRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}