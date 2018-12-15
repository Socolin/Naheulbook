using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions
{
    public static class ItemTemplateScenarioContextExtensions
    {
        private const string ItemTemplateSectionIdKey = "ItemTemplateSectionId";

        public static void SetItemTemplateSectionId(this ScenarioContext scenarioContext, int id)
        {
            scenarioContext.Set(id, ItemTemplateSectionIdKey);
        }

        public static int GetItemTemplateSectionId(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<int>(ItemTemplateSectionIdKey);
        }
    }
}