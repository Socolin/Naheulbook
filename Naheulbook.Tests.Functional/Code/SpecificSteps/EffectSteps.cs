using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.TestServices;
using Socolin.TestUtils.AutoFillTestObjects;
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
            var createEffectTypeRequest = AutoFill<CreateEffectTypeRequest>.One(AutoFillFlags.RandomizeString);
            var effectType = await _effectTestService.CreateEffectTypeAsync(createEffectTypeRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetEffectTypeId(effectType.Id);
        }

        [Given("an effect category")]
        public async Task GivenAnEffectCategory()
        {
            await GivenAnEffectType();

            var createEffectCategoryRequest = AutoFill<CreateEffectCategoryRequest>.One(AutoFillFlags.RandomizeString);
            createEffectCategoryRequest.TypeId = _scenarioContext.GetEffectTypeId();
            var effectCategory = await _effectTestService.CreateEffectCategoryAsync(createEffectCategoryRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetEffectCategoryId(effectCategory.Id);
        }

        [Given("an effect")]
        public async Task GivenAnEffect()
        {
            await GivenAnEffectCategory();

            var createEffectRequest = AutoFill<CreateEffectRequest>.One(AutoFillFlags.RandomizeString);
            createEffectRequest.CategoryId = _scenarioContext.GetEffectTypeId();
            createEffectRequest.Modifiers[0].Stat = "FO";
            createEffectRequest.Modifiers[1].Stat = "INT";
            createEffectRequest.Modifiers[2].Stat = "CHA";
            var effect = await _effectTestService.CreateEffectAsync(createEffectRequest, _scenarioContext.GetJwt());

            _scenarioContext.SetEffectId(effect.Id);
        }
    }
}