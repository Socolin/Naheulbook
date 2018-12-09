using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.TestServices;
using Naheulbook.Tests.Functional.Code.Utils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class EffectSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly EffectTestService _effectTestService;

        public EffectSteps(ScenarioContext scenarioContext, EffectTestService effectTestService)
        {
            _scenarioContext = scenarioContext;
            _effectTestService = effectTestService;
        }

        [Given("an effect type")]
        public async Task GivenAnEffectType()
        {
            var createEffectTypeRequest = new CreateEffectTypeRequest
            {
                Name = RngUtils.GetRandomString("some-name")
            };
            var effectType = await _effectTestService.CreateEffectTypeAsync(createEffectTypeRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetEffectTypeId(effectType.Id);
        }

        [Given("an effect category")]
        public async Task GivenAnEffectCategory()
        {
            await GivenAnEffectType();

            var createEffectCategoryRequest = new CreateEffectCategoryRequest
            {
                Name = RngUtils.GetRandomString("some-category-name"),
                TypeId = _scenarioContext.GetEffectTypeId(),
                Note = "some-note",
                DiceSize = 4,
                DiceCount = 3
            };
            var effectCategory = await _effectTestService.CreateEffectCategoryAsync(createEffectCategoryRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetEffectCategoryId(effectCategory.Id);
        }

        [Given("an effect")]
        public async Task GivenAnEffect()
        {
            await GivenAnEffectCategory();

            var createEffectRequest = new CreateEffectRequest
            {
                Name = RngUtils.GetRandomString("some-effect-name"),
                CategoryId = _scenarioContext.GetEffectCategoryId(),
                Description = "some-description",
                Modifiers = new List<CreateEffectModifierRequest>(),
            };
            var effect = await _effectTestService.CreateEffectAsync(createEffectRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetEffectId(effect.Id);
        }
    }
}