using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpecialityEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Flags { get; set; }

    public Guid JobId { get; set; }
    public virtual JobEntity Job { get; set; } = null!;

    private ICollection<SpecialityModifierEntity>? _modifiers;
    public ICollection<SpecialityModifierEntity> Modifiers { get => _modifiers.ThrowIfNotLoaded(); set => _modifiers = value; }

    private ICollection<SpecialitySpecialEntity>? _specials;
    public ICollection<SpecialitySpecialEntity> Specials { get => _specials.ThrowIfNotLoaded(); set => _specials = value; }
}