using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string TechName { get; set; }

        public int SectionId { get; set; }
        public ItemTemplateSection Section { get; set; }

        // FIXME: temp until ItemTemplate is configured in ef context
        [NotMapped]
        public Collection<ItemTemplate> ItemTemplates { get; set; }
    }
}