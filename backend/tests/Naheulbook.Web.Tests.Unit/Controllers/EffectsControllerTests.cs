using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class EffectsControllerTests
{
    private IEffectService _effectService;
    private IMapper _mapper;
    private EffectsController _effectsController;
    private NaheulbookExecutionContext _executionContext;

    [SetUp]
    public void SetUp()
    {
        _effectService = Substitute.For<IEffectService>();
        _mapper = Substitute.For<IMapper>();
        _effectsController = new EffectsController(_effectService, _mapper);
        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task PutEditEffect_CallEffectService()
    {
        var editEffectRequest = new EditEffectRequest {Name = string.Empty, DurationType = string.Empty, Modifiers = new List<StatModifierRequest>()};
        var expectedEffectResponse = new EffectResponse();
        var effect = new EffectEntity {Id = 42};

        _effectService.EditEffectAsync(_executionContext, 42, editEffectRequest)
            .Returns(effect);
        _mapper.Map<EffectResponse>(effect)
            .Returns(expectedEffectResponse);

        var result = await _effectsController.PutEditEffectAsync(_executionContext, 42, editEffectRequest);

        result.Value.Should().Be(expectedEffectResponse);
        await _effectService.Received(1)
            .EditEffectAsync(_executionContext, 42, editEffectRequest);
    }

    [Test]
    public async Task PutEditEffect_WhenCatchForbiddenAccessException_Return403()
    {
        var editEffectRequest = new EditEffectRequest {Name = string.Empty, DurationType = string.Empty, Modifiers = new List<StatModifierRequest>()};

        _effectService.EditEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<EditEffectRequest>())
            .Returns(Task.FromException<EffectEntity>(new ForbiddenAccessException()));

        Func<Task> act = () => _effectsController.PutEditEffectAsync(_executionContext, 42, editEffectRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Test]
    public async Task PutEditEffect_WhenCatchEffectNotFoundException_Return404()
    {
        var editEffectRequest = new EditEffectRequest {Name = string.Empty, DurationType = string.Empty, Modifiers = new List<StatModifierRequest>()};

        _effectService.EditEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<EditEffectRequest>())
            .Returns(Task.FromException<EffectEntity>(new EffectNotFoundException()));

        Func<Task> act = () => _effectsController.PutEditEffectAsync(_executionContext, 42, editEffectRequest);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Test]
    public async Task GetEffect_CallEffectService()
    {
        var expectedEffectResponse = new EffectResponse();
        var effect = new EffectEntity {Id = 42};

        _effectService.GetEffectAsync(42)
            .Returns(effect);
        _mapper.Map<EffectResponse>(effect)
            .Returns(expectedEffectResponse);

        var result = await _effectsController.GetEffectAsync(42);

        result.Value.Should().Be(expectedEffectResponse);
    }

    [Test]
    public async Task GetEffect_WhenCatchEffectNotFoundException_Return404()
    {
        _effectService.GetEffectAsync(Arg.Any<int>())
            .Returns(Task.FromException<EffectEntity>(new EffectNotFoundException()));

        Func<Task<ActionResult<EffectResponse>>> act = () => _effectsController.GetEffectAsync(42);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }
}