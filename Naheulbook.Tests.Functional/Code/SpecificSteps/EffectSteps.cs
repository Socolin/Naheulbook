using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.TestServices;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using Socolin.TestUtils.AutoFillTestObjects;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class EffectSteps
    {
        private readonly EffectTestService _effectTestService;
        private readonly TestDataUtil _testDataUtil;

        public EffectSteps(EffectTestService effectTestService, TestDataUtil testDataUtil)
        {
            _effectTestService = effectTestService;
            _testDataUtil = testDataUtil;
        }

        [Given("an effect type")]
        public void GivenAnEffectType()
        {
            _testDataUtil.AddEffectType();
        }

        [Given(@"(an|\d+) effects? categor(?:y|ies)")]
        public void GivenAnEffectCategory(string amount)
        {
            if (!_testDataUtil.Contains<EffectType>())
                _testDataUtil.AddEffectType();
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddEffectCategory();
        }

        [Given(@"(an|\d+) effects?")]
        public void GivenAnEffect(string amount)
        {
            if (!_testDataUtil.Contains<EffectType>())
                _testDataUtil.AddEffectType();
            if (!_testDataUtil.Contains<EffectCategory>())
                _testDataUtil.AddEffectCategory();

            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddEffect();
        }
    }
}