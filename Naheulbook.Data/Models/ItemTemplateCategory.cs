using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateCategory
    {
        public ItemTemplateCategory()
        {
            ItemTemplates = new HashSet<ItemTemplate>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string TechName { get; set; }

        public int SectionId { get; set; }
        public ItemTemplateSection Section { get; set; }

        public ICollection<ItemTemplate> ItemTemplates { get; set; }
    }
}