using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class ItemTemplateSectionsControllerTests
{
    private IItemTemplateSectionService _itemTemplateSectionService;
    private IMapper _mapper;
    private ItemTemplateSectionsController _itemTemplateSectionsController;
    private NaheulbookExecutionContext _executionContext;

    [SetUp]
    public void SetUp()
    {
        _itemTemplateSectionService = Substitute.For<IItemTemplateSectionService>();
        _mapper = Substitute.For<IMapper>();
        _itemTemplateSectionsController = new ItemTemplateSectionsController(_itemTemplateSectionService, _mapper);
        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task PostCreateSection_CallItemSectionService()
    {
        var createItemTemplateSectionRequest = new CreateItemTemplateSectionRequest {Name = string.Empty, Icon = string.Empty, Note = string.Empty, Specials = new List<string>()};
        var itemTemplateSection = new ItemTemplateSectionEntity();
        var itemTemplateSectionResponse = new ItemTemplateSectionResponse();

        _itemTemplateSectionService.CreateItemTemplateSectionAsync(_executionContext, createItemTemplateSectionRequest)
            .Returns(itemTemplateSection);
        _mapper.Map<ItemTemplateSectionResponse>(itemTemplateSection)
            .Returns(itemTemplateSectionResponse);

        var result = await _itemTemplateSectionsController.PostCreateSectionAsync(_executionContext, createItemTemplateSectionRequest);

        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(itemTemplateSectionResponse);
    }

    [Test]
    public async Task PostCreateSection_WhenCatchForbiddenAccessException_Return403()
    {
        var createItemTemplateSectionRequest = new CreateItemTemplateSectionRequest {Name = string.Empty, Icon = string.Empty, Note = string.Empty, Specials = new List<string>()};

        _itemTemplateSectionService.CreateItemTemplateSectionAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateItemTemplateSectionRequest>())
            .Returns(Task.FromException<ItemTemplateSectionEntity>(new ForbiddenAccessException()));

        var act = () => _itemTemplateSectionsController.PostCreateSectionAsync(_executionContext, createItemTemplateSectionRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}