using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class SkillSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public SkillSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given(@"(a|\d+) skills?")]
        public void GivenXSkills(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddSkill();
        }
    }
}