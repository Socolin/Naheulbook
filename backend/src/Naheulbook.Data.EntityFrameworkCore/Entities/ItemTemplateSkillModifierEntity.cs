using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class ItemTemplateSkillModifierEntity
{
    public int Id { get; set; }

    public short Value { get; set; }

    public Guid ItemTemplateId { get; set; }
    private ItemTemplateEntity? _itemTemplate;
    public ItemTemplateEntity ItemTemplate { get => _itemTemplate.ThrowIfNotLoaded(); set => _itemTemplate = value; }

    public Guid SkillId { get; set; }
    private SkillEntity? _skill;
    public SkillEntity Skill { get => _skill.ThrowIfNotLoaded(); set => _skill = value; }
}