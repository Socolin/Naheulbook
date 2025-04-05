using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateSlotEntity
{
    public Guid ItemTemplateId { get; set; }
    private ItemTemplateEntity? _itemTemplate;
    public ItemTemplateEntity ItemTemplate { get => _itemTemplate.ThrowIfNotLoaded(); set => _itemTemplate = value; }

    public int SlotId { get; set; }
    private SlotEntity? _slot;
    public SlotEntity Slot { get => _slot.ThrowIfNotLoaded(); set => _slot = value; }
}