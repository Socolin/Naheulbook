using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class CharacterAptitudeEntity
{
    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

    public Guid AptitudeId { get; set; }
    private AptitudeEntity? _aptitude;
    public AptitudeEntity Aptitude { get => _aptitude.ThrowIfNotLoaded(); set => _aptitude = value; }

    public int Count { get; set; } = 1;
    public bool Active { get; set; }
}