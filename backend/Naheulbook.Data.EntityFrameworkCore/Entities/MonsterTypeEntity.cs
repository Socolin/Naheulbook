using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterTypeEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    private ICollection<MonsterSubCategoryEntity>? _subCategories;
    public ICollection<MonsterSubCategoryEntity> SubCategories { get => _subCategories.ThrowIfNotLoaded(); set => _subCategories = value; }
}