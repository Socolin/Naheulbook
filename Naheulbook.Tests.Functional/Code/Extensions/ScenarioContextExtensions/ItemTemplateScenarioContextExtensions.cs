using Naheulbook.Data.Models;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions
{
    public static class ItemTemplateScenarioContextExtensions
    {
        private const string ItemSlotKey = "ItemSlot";
        private const string ItemTemplateSectionIdKey = "ItemTemplateSectionId";
        private const string ItemTemplateCategoryIdKey = "ItemTemplateCategoryId";

        public static void SetItemTemplateSectionId(this ScenarioContext scenarioContext, int id)
        {
            scenarioContext.Set(id, ItemTemplateSectionIdKey);
        }

        public static int GetItemTemplateSectionId(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<int>(ItemTemplateSectionIdKey);
        }

        public static void SetItemTemplateCategoryId(this ScenarioContext scenarioContext, int id)
        {
            scenarioContext.Set(id, ItemTemplateCategoryIdKey);
        }

        public static int GetItemTemplateCategoryId(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<int>(ItemTemplateCategoryIdKey);
        }

        public static void SetItemSlot(this ScenarioContext scenarioContext, Slot slot)
        {
            scenarioContext.Set(slot, ItemSlotKey);
        }

        public static Slot GetItemSlot(this ScenarioContext scenarioContext)
        {
            return scenarioContext.Get<Slot>(ItemSlotKey);
        }
    }
}