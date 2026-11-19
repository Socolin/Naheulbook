using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterMasteryEntity
{
    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

    public Guid MasteryId { get; set; }
    private MasteryEntity? _mastery;
    public MasteryEntity Mastery { get => _mastery.ThrowIfNotLoaded(); set => _mastery = value; }
}
