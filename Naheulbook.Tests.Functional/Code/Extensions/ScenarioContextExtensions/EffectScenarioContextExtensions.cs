using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions
{
    public static class EffectScenarioContextExtensions
    {
        private const string EffectTypeIdKey = "EffectTypeId";
        private const string EffectCategoryIdKey = "EffectCategoryId";
        private const string EffectIdKey = "EffectId";

        public static void SetEffectTypeId(this ScenarioContext scenarioContext, int id)
        {
            scenarioContext.Set(id, EffectTypeIdKey);
        }

        public static int GetEffectTypeId(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<int>(EffectTypeIdKey);
        }

        public static void SetEffectCategoryId(this ScenarioContext scenarioContext, int id)
        {
            scenarioContext.Set(id, EffectCategoryIdKey);
        }

        public static int GetEffectCategoryId(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<int>(EffectCategoryIdKey);
        }

        public static void SetEffectId(this ScenarioContext scenarioContext, int id)
        {
            scenarioContext.Set(id, EffectIdKey);
        }

        public static int GetEffectId(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<int>(EffectIdKey);
        }
    }
}