using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateSection
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Note { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public string Special { get; set; } = null!;

        [NotMapped]
        public ICollection<ItemTemplateSubCategory> SubCategories { get; set; } = null!;
    }
}