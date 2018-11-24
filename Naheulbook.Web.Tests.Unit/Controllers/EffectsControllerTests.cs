using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Web.Controllers;
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

        [SetUp]
        public void SetUp()
        {
            _effectService = Substitute.For<IEffectService>();
            _mapper = Substitute.For<IMapper>();
            _effectsController = new EffectsController(_effectService, _mapper);
        }

        [Test]
        public async Task CanGetEffects()
        {
            var effects = new List<Effect>();
            var expectedResponse = new List<EffectResponse>();

            _effectService.GetEffectsByCategoryAsync(42)
                .Returns(effects);
            _mapper.Map<List<EffectResponse>>(effects)
                .Returns(expectedResponse);

            var result = await _effectsController.GetEffectsAsync(42);

            result.Value.Should().BeSameAs(expectedResponse);
        }
    }
}