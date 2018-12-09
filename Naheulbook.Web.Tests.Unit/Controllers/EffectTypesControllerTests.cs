using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class EffectTypesControllerTests : BaseControllerTests
    {
        private IEffectService _effectService;
        private IMapper _mapper;
        private EffectTypesController _effectTypesController;

        [SetUp]
        public void SetUp()
        {
            _effectService = Substitute.For<IEffectService>();
            _mapper = Substitute.For<IMapper>();
            _effectTypesController = new EffectTypesController(_effectService, _mapper);
            MockHttpContext(_effectTypesController);
        }

        [Test]
        public async Task PostCreateType_CallEffectService()
        {
            var createEffectTypeRequest = new CreateEffectTypeRequest();
            var effectType = new EffectType();
            var effectTypeResponse = new EffectTypeResponse();

            _effectService.CreateEffectTypeAsync(ExecutionContext, createEffectTypeRequest)
                .Returns(effectType);
            _mapper.Map<EffectTypeResponse>(effectType)
                .Returns(effectTypeResponse);

            var result = await _effectTypesController.PostCreateTypeAsync(createEffectTypeRequest);

            var jsonResult = ((JsonResult) result.Result);
            jsonResult.StatusCode.Should().Be(201);
            jsonResult.Value.Should().Be(effectTypeResponse);
        }
    }
}