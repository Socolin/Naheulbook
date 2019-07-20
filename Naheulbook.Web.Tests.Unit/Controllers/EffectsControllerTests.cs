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
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
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
            var editEffectRequest = new EditEffectRequest();
            var expectedEffectResponse = new EffectResponse();
            var effect = new Effect {Id = 42};

            _mapper.Map<EffectResponse>(effect)
                .Returns(expectedEffectResponse);

            var result = await _effectsController.PutEditEffectAsync(_executionContext, 42, editEffectRequest);

            result.StatusCode.Should().Be(204);
            await _effectService.Received(1)
                .EditEffectAsync(_executionContext, 42, editEffectRequest);
        }

        [Test]
        public void PutEditEffect_WhenCatchForbiddenAccessException_Return403()
        {
            _effectService.EditEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<EditEffectRequest>())
                .Returns(Task.FromException<Effect>(new ForbiddenAccessException()));

            Func<Task<StatusCodeResult>> act = () => _effectsController.PutEditEffectAsync(_executionContext, 42, new EditEffectRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public void PutEditEffect_WhenCatchEffectNotFoundException_Return404()
        {
            _effectService.EditEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<EditEffectRequest>())
                .Returns(Task.FromException<Effect>(new EffectNotFoundException()));

            Func<Task<StatusCodeResult>> act = () => _effectsController.PutEditEffectAsync(_executionContext, 42, new EditEffectRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetEffect_CallEffectService()
        {
            var expectedEffectResponse = new EffectResponse();
            var effect = new Effect {Id = 42};

            _effectService.GetEffectAsync(42)
                .Returns(effect);
            _mapper.Map<EffectResponse>(effect)
                .Returns(expectedEffectResponse);

            var result = await _effectsController.GetEffectAsync(42);

            result.Value.Should().Be(expectedEffectResponse);
        }

        [Test]
        public void GetEffect_WhenCatchEffectNotFoundException_Return404()
        {
            _effectService.GetEffectAsync(Arg.Any<int>())
                .Returns(Task.FromException<Effect>(new EffectNotFoundException()));

            Func<Task<ActionResult<EffectResponse>>> act = () => _effectsController.GetEffectAsync(42);

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}