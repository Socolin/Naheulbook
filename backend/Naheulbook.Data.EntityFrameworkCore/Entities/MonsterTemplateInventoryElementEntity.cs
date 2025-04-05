using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class MonsterTemplateInventoryElementEntity
{
    public int Id { get; set; }
    public float Chance { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }

    public Guid ItemTemplateId { get; set; }
    private ItemTemplateEntity? _itemTemplate;
    public ItemTemplateEntity ItemTemplate { get => _itemTemplate.ThrowIfNotLoaded(); set => _itemTemplate = value; }

    public int MonsterTemplateId { get; set; }
    private MonsterTemplateEntity? _monsterTemplate;
    public MonsterTemplateEntity MonsterTemplate { get => _monsterTemplate.ThrowIfNotLoaded(); set => _monsterTemplate = value; }
}