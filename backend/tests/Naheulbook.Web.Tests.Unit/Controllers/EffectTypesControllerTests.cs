using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class EffectTypesControllerTests
{
    private IEffectService _effectService;
    private IEffectMapper _effectMapper;
    private EffectTypesController _effectTypesController;
    private NaheulbookExecutionContext _executionContext;

    [SetUp]
    public void SetUp()
    {
        _effectService = Substitute.For<IEffectService>();
        _effectMapper = Substitute.For<IEffectMapper>();
        _effectTypesController = new EffectTypesController(_effectService, _effectMapper);
        _executionContext = new NaheulbookExecutionContext();
    }

    [Test]
    public async Task PostCreateType_CallEffectService()
    {
        var createEffectTypeRequest = new CreateEffectTypeRequest {Name = string.Empty};
        var effectType = new EffectTypeEntity();
        var effectTypeResponse = new EffectTypeResponse();

        _effectService.CreateEffectTypeAsync(_executionContext, createEffectTypeRequest)
            .Returns(effectType);
        _effectMapper.ToResponse(effectType)
            .Returns(effectTypeResponse);

        var result = await _effectTypesController.PostCreateTypeAsync(_executionContext, createEffectTypeRequest, CancellationToken.None);

        result.StatusCode.Should().Be(201);
        result.Value.Should().Be(effectTypeResponse);
    }

    [Test]
    public async Task PostCreateType_WhenCatchForbiddenAccessException_Return403()
    {
        var request = new CreateEffectTypeRequest {Name = string.Empty};

        _effectService.CreateEffectTypeAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateEffectTypeRequest>())
            .Returns(Task.FromException<EffectTypeEntity>(new ForbiddenAccessException()));

        Func<Task<JsonResult>> act = () => _effectTypesController.PostCreateTypeAsync(_executionContext, request, CancellationToken.None);

        (await act.Should().ThrowAsync<HttpErrorException>()).Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }
}