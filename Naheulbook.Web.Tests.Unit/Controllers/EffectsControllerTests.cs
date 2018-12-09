using System;
using System.Collections.Generic;
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
    public class EffectsControllerTests : BaseControllerTests
    {
        private IEffectService _effectService;
        private IMapper _mapper;
        private EffectsController _effectsController;

        [SetUp]
        public void SetUp()
        {
            _effectService = Substitute.For<IEffectService>();
            _mapper = Substitute.For<IMapper>();
            _effectsController = new EffectsController(_effectService, _mapper);
            MockHttpContext(_effectsController);
        }

        [Test]
        public async Task PostCreateEffect_CallEffectService()
        {
            var createEffectRequest = CreateEffectRequest();
            var expectedEffectResponse = new EffectResponse();
            var effect = new Effect {Id = 42};

            _effectService.CreateEffectAsync(ExecutionContext, createEffectRequest)
                .Returns(effect);
            _mapper.Map<EffectResponse>(effect)
                .Returns(expectedEffectResponse);

            var result = await _effectsController.PostCreateEffectAsync(createEffectRequest);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(expectedEffectResponse);
        }

        [Test]
        public void PostCreateEffect_WhenCatchForbiddenAccessException_Return403()
        {
            _effectService.CreateEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateEffectRequest>())
                .Returns(Task.FromException<Effect>(new ForbiddenAccessException()));

            Func<Task<JsonResult>> act = () => _effectsController.PostCreateEffectAsync(new CreateEffectRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        private static CreateEffectRequest CreateEffectRequest()
        {
            return new CreateEffectRequest()
            {
                Name = "some-effect-name",
                CategoryId = 1,
                CombatCount = 2,
                Description = "some-description",
                Dice = 3,
                Duration = "some-duration",
                TimeDuration = 4,
                Modifiers = new List<CreateEffectModifierRequest>
                {
                    new CreateEffectModifierRequest
                    {
                        Value = 5,
                        Stat = "some-stat",
                        Type = "some-type"
                    }
                }
            };
        }
    }
}