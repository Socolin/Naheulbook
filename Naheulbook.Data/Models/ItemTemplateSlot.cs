namespace Naheulbook.Data.Models
{
    public class ItemTemplateSlot
    {
        public int ItemTemplateId { get; set; }
        public ItemTemplate ItemTemplate { get; set; } = null!;

        public int SlotId { get; set; }
        public Slot Slot { get; set; } = null!;
    }
}