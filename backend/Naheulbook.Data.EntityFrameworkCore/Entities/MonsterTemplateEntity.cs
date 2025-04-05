using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterTemplateEntity
{
    public int Id { get; set; }

    public string Data { get; set; } = null!;
    public string Name { get; set; } = null!;

    public int SubCategoryId { get; set; }
    private MonsterSubCategoryEntity? _subCategory;
    public MonsterSubCategoryEntity SubCategory { get => _subCategory.ThrowIfNotLoaded(); set => _subCategory = value; }

    private ICollection<MonsterTemplateInventoryElementEntity>? _items;
    public ICollection<MonsterTemplateInventoryElementEntity> Items { get => _items.ThrowIfNotLoaded(); set => _items = value; }
}