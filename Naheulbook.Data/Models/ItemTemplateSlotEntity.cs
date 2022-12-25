using System;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateSlotEntity
    {
        public Guid ItemTemplateId { get; set; }
        public ItemTemplateEntity ItemTemplate { get; set; } = null!;

        public int SlotId { get; set; }
        public SlotEntity Slot { get; set; } = null!;
    }
}