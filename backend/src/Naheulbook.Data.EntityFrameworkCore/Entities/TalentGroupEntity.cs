using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class TalentGroupEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    private ICollection<TalentEntity>? _talents;
    public ICollection<TalentEntity> Talents { get => _talents.ThrowIfNotLoaded(); set => _talents = value; }
}
