using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    public class EffectControllerTests
    {
        private IEffectService _effectService;
        private IMapper _mapper;
        private EffectCategoriesController _effectCategoriesController;
        private NaheulbookExecutionContext _executionContext;

        [SetUp]
        public void SetUp()
        {
            _effectService = Substitute.For<IEffectService>();
            _mapper = Substitute.For<IMapper>();
            _effectCategoriesController = new EffectCategoriesController(_effectService, _mapper);
            _executionContext = new NaheulbookExecutionContext();
        }

        [Test]
        public async Task PostCreateEffect_CallEffectService()
        {
            const int categoryId = 12;
            var createEffectRequest = new CreateEffectRequest();
            var expectedEffectResponse = new EffectResponse();
            var effect = new Effect {Id = 42};

            _effectService.CreateEffectAsync(_executionContext, categoryId, createEffectRequest)
                .Returns(effect);
            _mapper.Map<EffectResponse>(effect)
                .Returns(expectedEffectResponse);

            var result = await _effectCategoriesController.PostCreateEffectAsync(_executionContext, categoryId, createEffectRequest);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(expectedEffectResponse);
        }

        [Test]
        public void PostCreateEffect_WhenCatchForbiddenAccessException_Return403()
        {
            _effectService.CreateEffectAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<int>(), Arg.Any<CreateEffectRequest>())
                .Returns(Task.FromException<Effect>(new ForbiddenAccessException()));

            Func<Task> act = () => _effectCategoriesController.PostCreateEffectAsync(_executionContext, 12, new CreateEffectRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
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

            _effectService.CreateEffectCategoryAsync(_executionContext, request)
                .Returns(effectCategory);
            _mapper.Map<EffectCategoryResponse>(effectCategory)
                .Returns(expectedEffectCategoryResponse);

            var result = await _effectCategoriesController.PostCreateCategoryAsync(_executionContext, request);

            result.StatusCode.Should().Be(201);
            result.Value.Should().Be(expectedEffectCategoryResponse);
        }

        [Test]
        public void PostCreateCategory_WhenCatchForbiddenAccessException_Return403()
        {
            _effectService.CreateEffectCategoryAsync(Arg.Any<NaheulbookExecutionContext>(), Arg.Any<CreateEffectCategoryRequest>())
                .Returns(Task.FromException<EffectCategory>(new ForbiddenAccessException()));

            Func<Task<JsonResult>> act = () => _effectCategoriesController.PostCreateCategoryAsync(_executionContext, CreateEffectCategoryRequest());

            act.Should().Throw<HttpErrorException>().Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
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