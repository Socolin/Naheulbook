using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddSlot(Action<Slot> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateSlot(), customizer);
        }

        public TestDataUtil AddItemTemplateSection(Action<ItemTemplateSection> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplateSection(), customizer);
        }

        public TestDataUtil AddItemTemplateCategory(Action<ItemTemplateCategory> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplateCategory(GetLast<ItemTemplateSection>()), customizer);
        }

        public TestDataUtil AddItemTemplate(Action<ItemTemplate> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplate(GetLast<ItemTemplateCategory>()), customizer);
        }
    }
}