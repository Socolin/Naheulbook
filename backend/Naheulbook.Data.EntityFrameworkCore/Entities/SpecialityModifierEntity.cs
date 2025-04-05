using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class SpecialityModifierEntity
{
    public int Id { get; set; }
    public string Stat { get; set; } = null!;
    public int Value { get; set; }

    public Guid SpecialityId { get; set; }
    private SpecialityEntity? _speciality;
    public SpecialityEntity Speciality { get => _speciality.ThrowIfNotLoaded(); set => _speciality = value; }
}