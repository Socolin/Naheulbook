using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class EffectSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public EffectSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("an effect type")]
        public void GivenAnEffectType()
        {
            _testDataUtil.AddEffectType();
        }

        [Given(@"(an|\d+) effects? sub-categor(?:y|ies)")]
        public void GivenAnEffectSubCategory(string amount)
        {
            if (!_testDataUtil.Contains<EffectTypeEntity>())
                _testDataUtil.AddEffectType();
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddEffectSubCategory();
        }

        [Given(@"(an|\d+) effects?")]
        public void GivenAnEffect(string amount)
        {
            if (!_testDataUtil.Contains<EffectTypeEntity>())
                _testDataUtil.AddEffectType();
            if (!_testDataUtil.Contains<EffectSubCategoryEntity>())
                _testDataUtil.AddEffectSubCategory();

            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddEffect();
        }
    }
}