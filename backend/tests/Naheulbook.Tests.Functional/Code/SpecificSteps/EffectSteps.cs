using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class EffectSteps(TestDataUtil testDataUtil)
{
    [Given("an effect type")]
    public void GivenAnEffectType()
    {
        testDataUtil.AddEffectType();
    }

    [Given(@"(an|\d+) effects? sub-categor(?:y|ies)")]
    public void GivenAnEffectSubCategory(string amount)
    {
        if (!testDataUtil.Contains<EffectTypeEntity>())
            testDataUtil.AddEffectType();
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddEffectSubCategory();
    }

    [Given(@"(an|\d+) effects?")]
    public void GivenAnEffect(string amount)
    {
        if (!testDataUtil.Contains<EffectTypeEntity>())
            testDataUtil.AddEffectType();
        if (!testDataUtil.Contains<EffectSubCategoryEntity>())
            testDataUtil.AddEffectSubCategory();

        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddEffect()
                .AddEffectModifier();
    }
}