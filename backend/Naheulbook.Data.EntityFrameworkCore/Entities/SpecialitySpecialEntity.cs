using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class SpecialitySpecialEntity
{
    public int Id { get; set; }
    public bool IsBonus { get; set; }
    public string Description { get; set; } = null!;
    public string? Flags { get; set; }

    public Guid SpecialityId { get; set; }
    private SpecialityEntity? _speciality;
    public SpecialityEntity Speciality { get => _speciality.ThrowIfNotLoaded(); set => _speciality = value; }
}