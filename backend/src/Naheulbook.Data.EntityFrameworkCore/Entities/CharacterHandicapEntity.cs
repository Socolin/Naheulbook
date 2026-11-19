using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterHandicapEntity
{
    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

    public Guid HandicapId { get; set; }
    private HandicapEntity? _handicap;
    public HandicapEntity Handicap { get => _handicap.ThrowIfNotLoaded(); set => _handicap = value; }
}
