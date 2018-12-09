using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class EffectsControllerTests : BaseControllerTests
    {
        private IEffectService _effectService;
        private IMapper _mapper;
        private EffectCategoriesController _effectCategoriesController;

        [SetUp]
        public void SetUp()
        {
            _effectService = Substitute.For<IEffectService>();
            _mapper = Substitute.For<IMapper>();
            _effectCategoriesController = new EffectCategoriesController(_effectService, _mapper);
            MockHttpContext(_effectCategoriesController);
        }

        [Test]
        public async Task GetEffects_CallEffectService()
        {
            var effects = new List<Effect>();
            var expectedResponse = new List<EffectResponse>();

            _effectService.GetEffectsByCategoryAsync(42)
                .Returns(effects);
            _mapper.Map<List<EffectResponse>>(effects)
                .Returns(expectedResponse);

            var result = await _effectCategoriesController.GetEffectsAsync(42);

            result.Value.Should().BeSameAs(expectedResponse);
        }

        [Test]
        public async Task PostCreateCategory_CallEffectService()
        {
            var request = CreateEffectCategoryRequest();
            var effectCategory = new EffectCategory();
            var expectedEffectCategoryResponse = new EffectCategoryResponse();

            _effectService.CreateEffectCategoryAsync(ExecutionContext, "some-name", 24, 25, 26, "some-note")
                .Returns(effectCategory);
            _mapper.Map<EffectCategoryResponse>(effectCategory)
                .Returns(expectedEffectCategoryResponse);

            var result = await _effectCategoriesController.PostCreateCategoryAsync(request);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(expectedEffectCategoryResponse);
        }

        private static CreateEffectCategoryRequest CreateEffectCategoryRequest()
        {
            return new CreateEffectCategoryRequest
            {
                Name = "some-name",
                TypeId = 24,
                DiceSize = 25,
                DiceCount = 26,
                Note = "some-note"
            };
        }
    }
}