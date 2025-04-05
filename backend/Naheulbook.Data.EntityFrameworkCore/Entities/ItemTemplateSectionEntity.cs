using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateSectionEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Note { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Special { get; set; } = null!;

    private ICollection<ItemTemplateSubCategoryEntity>? _subCategories;
    public ICollection<ItemTemplateSubCategoryEntity> SubCategories { get => _subCategories.ThrowIfNotLoaded(); set => _subCategories = value; }
}