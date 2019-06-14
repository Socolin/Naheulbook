using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class MonsterSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public MonsterSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a monster category type")]
        public void GivenAMonsterCategoryType()
        {
            _testDataUtil.AddMonsterType();
        }

        [Given("a monster category")]
        public void GivenAMonsterCategory()
        {
            _testDataUtil.AddMonsterCategory();
        }

        [Given("a monster trait")]
        public void GivenAMonsterTrait()
        {
            _testDataUtil.AddMonsterTrait();
        }
    }
}