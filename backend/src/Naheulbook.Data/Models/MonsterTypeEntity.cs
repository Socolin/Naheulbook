using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class MonsterTypeEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    private ICollection<MonsterSubCategoryEntity>? _subCategories;
    public ICollection<MonsterSubCategoryEntity> SubCategories { get => _subCategories.ThrowIfNotLoaded(); set => _subCategories = value; }
}