using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateSubCategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Note { get; set; } = null!;
    public string TechName { get; set; } = null!;

    public int SectionId { get; set; }
    private ItemTemplateSectionEntity? _section;
    public ItemTemplateSectionEntity Section { get => _section.ThrowIfNotLoaded(); set => _section = value; }

    private ICollection<ItemTemplateEntity>? _itemTemplates;
    public ICollection<ItemTemplateEntity> ItemTemplates { get => _itemTemplates.ThrowIfNotLoaded(); set => _itemTemplates = value; }
}