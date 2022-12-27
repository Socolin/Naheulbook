using System;
using System.Collections.Generic;
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

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class EffectControllerTests
{
    private IEffectService _effectService;
    private IMapper _mapper;
    private EffectSubCategoriesController _effectSubCategoriesController;
    private NaheulbookExecutionContext _executionContext;

    [SetUp]
    public void SetUp()
    {
        _effectService = Substitute.For<IEffectService>();
        _mapper = Substitute.For<IMapper>();
        _effectSubCategoriesController = new EffectSubCategoriesController(_effectService, _mapper);
        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task PostCreateEffect_CallEffectService()
    {
        const int subCategoryId = 12;
        var createEffectRequest = new CreateEffectRequest();
        var expectedEffectResponse = new EffectResponse();
        var effect = new EffectEntity {Id = 42};

        _effectService.CreateEffectAsync(_executionContext, subCategoryId, createEffectRequest)
            .Returns(effect);
        _mapper.Map<EffectResponse>(effect)
            .Returns(expectedEffectResponse);

        var result = await _effectSubCategoriesController.PostCreateEffectAsync(_executionContext, subCategoryId, createEffectRequest);

        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(expectedEffectResponse);
    }

    [Test]
    public async Task PostCreateEffect_WhenCatchForbiddenAccessException_Return403()
    {
        _effectService.CreateEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateEffectRequest>())
            .Returns(Task.FromException<EffectEntity>(new ForbiddenAccessException()));

        Func<Task> act = () => _effectSubCategoriesController.PostCreateEffectAsync(_executionContext, 12, new CreateEffectRequest());

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Test]
    public async Task GetEffects_CallEffectService()
    {
        var effects = new List<EffectEntity>();
        var expectedResponse = new List<EffectResponse>();

        _effectService.GetEffectsBySubCategoryAsync(42)
            .Returns(effects);
        _mapper.Map<List<EffectResponse>>(effects)
            .Returns(expectedResponse);

        var result = await _effectSubCategoriesController.GetEffectsAsync(42);

        result.Value.Should().BeSameAs(expectedResponse);
    }

    [Test]
    public async Task PostCreateCategory_CallEffectService()
    {
        var request = CreateEffectSubCategoryRequest();
        var effectSubCategory = new EffectSubCategoryEntity();
        var expectedEffectSubCategoryResponse = new EffectSubCategoryResponse();

        _effectService.CreateEffectSubCategoryAsync(_executionContext, request)
            .Returns(effectSubCategory);
        _mapper.Map<EffectSubCategoryResponse>(effectSubCategory)
            .Returns(expectedEffectSubCategoryResponse);

        var result = await _effectSubCategoriesController.PostCreateEffectSubCategoryAsync(_executionContext, request);

        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(expectedEffectSubCategoryResponse);
    }

    [Test]
    public async Task PostCreateCategory_WhenCatchForbiddenAccessException_Return403()
    {
        _effectService.CreateEffectSubCategoryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateEffectSubCategoryRequest>())
            .Returns(Task.FromException<EffectSubCategoryEntity>(new ForbiddenAccessException()));

        Func<Task> act = () => _effectSubCategoriesController.PostCreateEffectSubCategoryAsync(_executionContext, CreateEffectSubCategoryRequest());

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    private static CreateEffectSubCategoryRequest CreateEffectSubCategoryRequest()
    {
        return new CreateEffectSubCategoryRequest
        {
            Name = "some-name",
            TypeId = 24,
            DiceSize = 25,
            DiceCount = 26,
            Note = "some-note"
        };
    }
}