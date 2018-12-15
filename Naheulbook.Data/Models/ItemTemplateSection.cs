using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateSection
    {
        public ItemTemplateSection()
        {
            Categories = new HashSet<ItemTemplateCategory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Special { get; set; }

        // FIXME: until its mapped
        [NotMapped]
        public ICollection<ItemTemplateCategory> Categories { get; set; }
    }
}