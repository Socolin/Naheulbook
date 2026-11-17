using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class EffectControllerTests
{
    private IEffectService _effectService;
    private IEffectMapper _effectMapper;
    private EffectSubCategoriesController _effectSubCategoriesController;
    private NaheulbookExecutionContext _executionContext;

    [SetUp]
    public void SetUp()
    {
        _effectService = Substitute.For<IEffectService>();
        _effectMapper = Substitute.For<IEffectMapper>();
        _effectSubCategoriesController = new EffectSubCategoriesController(_effectService, _effectMapper);
        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task PostCreateEffect_CallEffectService()
    {
        const int subCategoryId = 12;
        var createEffectRequest = new CreateEffectRequest {Name = string.Empty, DurationType = string.Empty, Modifiers = new List<StatModifierRequest>()};
        var expectedEffectResponse = new EffectResponse();
        var effect = new EffectEntity {Id = 42};

        _effectService.CreateEffectAsync(_executionContext, subCategoryId, createEffectRequest)
            .Returns(effect);
        _effectMapper.ToResponse(effect)
            .Returns(expectedEffectResponse);

        var result = await _effectSubCategoriesController.PostCreateEffectAsync(_executionContext, subCategoryId, createEffectRequest);

        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(expectedEffectResponse);
    }

    [Test]
    public async Task PostCreateEffect_WhenCatchForbiddenAccessException_Return403()
    {
        var createEffectRequest = new CreateEffectRequest {Name = string.Empty, DurationType = string.Empty, Modifiers = new List<StatModifierRequest>()};

        _effectService.CreateEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateEffectRequest>())
            .Returns(Task.FromException<EffectEntity>(new ForbiddenAccessException()));

        Func<Task> act = () => _effectSubCategoriesController.PostCreateEffectAsync(_executionContext, 12, createEffectRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Test]
    public async Task GetEffects_CallEffectService()
    {
        var entity1 = new EffectEntity();
        var entity2 = new EffectEntity();
        var effects = new List<EffectEntity> {entity1, entity2};
        var response1 = new EffectResponse();
        var response2 = new EffectResponse();

        _effectService.GetEffectsBySubCategoryAsync(42)
            .Returns(effects);
        _effectMapper.ToResponse(entity1)
            .Returns(response1);
        _effectMapper.ToResponse(entity2)
            .Returns(response2);

        var result = await _effectSubCategoriesController.GetEffectsAsync(42);

        result.Value.Should().Equal(response1, response2);
    }

    [Test]
    public async Task PostCreateCategory_CallEffectService()
    {
        var request = CreateEffectSubCategoryRequest();
        var effectSubCategory = new EffectSubCategoryEntity();
        var expectedEffectSubCategoryResponse = new EffectSubCategoryResponse();

        _effectService.CreateEffectSubCategoryAsync(_executionContext, request)
            .Returns(effectSubCategory);
        _effectMapper.ToResponse(effectSubCategory)
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
            Note = "some-note",
        };
    }
}