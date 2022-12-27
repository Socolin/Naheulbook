using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class ItemTemplateRequirementEntity
{
    public int Id { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }

    public Guid ItemTemplateId { get; set; }
    private ItemTemplateEntity? _itemTemplate;
    public ItemTemplateEntity ItemTemplate { get => _itemTemplate.ThrowIfNotLoaded(); set => _itemTemplate = value; }

    public string StatName { get; set; } = null!;
    private StatEntity? _stat;
    public StatEntity Stat { get => _stat.ThrowIfNotLoaded(); set => _stat = value; }
}