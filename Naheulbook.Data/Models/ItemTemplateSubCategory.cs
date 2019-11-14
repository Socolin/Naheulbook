using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateSubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Note { get; set; } = null!;
        public string TechName { get; set; } = null!;

        public int SectionId { get; set; }
        public ItemTemplateSection Section { get; set; } = null!;

        public ICollection<ItemTemplate> ItemTemplates { get; set; } = null!;
    }
}