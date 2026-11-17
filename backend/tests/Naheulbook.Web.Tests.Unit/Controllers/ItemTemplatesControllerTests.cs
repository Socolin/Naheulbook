using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class ItemTemplatesControllerTests
{
    private IItemTemplateService _itemTemplateService;
    private IItemTemplateMapper _itemTemplateMapper;
    private ItemTemplatesController _itemTemplatesController;
    private NaheulbookExecutionContext _executionContext;

    [SetUp]
    public void SetUp()
    {
        _itemTemplateService = Substitute.For<IItemTemplateService>();
        _itemTemplateMapper = Substitute.For<IItemTemplateMapper>();
        _itemTemplatesController = new ItemTemplatesController(_itemTemplateService, _itemTemplateMapper);
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
        _itemTemplateMapper.ToResponse(itemTemplate)
            .Returns(itemTemplateResponse);

        var result = await _itemTemplatesController.GetItemTemplateAsync(itemTemplateId, CancellationToken.None);

        result.Value.Should().Be(itemTemplateResponse);
    }

    [Test]
    public async Task GetItemTemplateAsync_Return404_WhenItemTemplateNotFoundIsThrow()
    {
        var itemTemplateId = Guid.NewGuid();
        _itemTemplateService.GetItemTemplateAsync(Arg.Any<Guid>())
            .Throws(new ItemTemplateNotFoundException(itemTemplateId));

        Func<Task> act = () => _itemTemplatesController.GetItemTemplateAsync(itemTemplateId, CancellationToken.None);

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
        _itemTemplateMapper.ToResponse(itemTemplate)
            .Returns(itemTemplateResponse);

        var result = await _itemTemplatesController.PutItemTemplateAsync(_executionContext, itemTemplateId, itemTemplateRequest, CancellationToken.None);

        result.Value.Should().Be(itemTemplateResponse);
    }

    [Test]
    public async Task PutItemTemplateAsync_Return404_WhenItemTemplateNotFoundIsThrow()
    {
        var itemTemplateId = Guid.NewGuid();
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};

        _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<Guid>(), Arg.Any<ItemTemplateRequest>())
            .Throws(new ItemTemplateNotFoundException(itemTemplateId));

        Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, itemTemplateId, itemTemplateRequest, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task PutItemTemplateAsync_Return403_WhenNotAllowed()
    {
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};

        _itemTemplateService.EditItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<Guid>(), Arg.Any<ItemTemplateRequest>())
            .Throws(new ForbiddenAccessException());

        Func<Task> act = () => _itemTemplatesController.PutItemTemplateAsync(_executionContext, Guid.NewGuid(), itemTemplateRequest, CancellationToken.None);

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
        _itemTemplateMapper.ToResponse(itemTemplate)
            .Returns(itemTemplateResponse);

        var result = await _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, itemTemplateRequest, CancellationToken.None);

        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(itemTemplateResponse);
    }

    [Test]
    public async Task PostCreateItemTemplate_WhenCatchForbiddenAccessException_Return403()
    {
        var itemTemplateRequest = new ItemTemplateRequest {Name = string.Empty, Source = string.Empty, Data = new JObject()};

        _itemTemplateService.CreateItemTemplateAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<ItemTemplateRequest>())
            .Returns(Task.FromException<ItemTemplateEntity>(new ForbiddenAccessException()));

        Func<Task<JsonResult>> act = () => _itemTemplatesController.PostCreateItemTemplateAsync(_executionContext, itemTemplateRequest, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}