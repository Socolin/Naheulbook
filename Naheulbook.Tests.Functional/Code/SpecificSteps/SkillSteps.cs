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

        [Given("a skill")]
        public void GivenASkill()
        {
            _testDataUtil.AddSkill();
        }
    }
}