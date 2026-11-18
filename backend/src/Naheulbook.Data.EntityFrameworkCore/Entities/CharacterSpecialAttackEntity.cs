using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterSpecialAttackEntity
{
    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

    public Guid SpecialAttackId { get; set; }
    private SpecialAttackEntity? _specialAttack;
    public SpecialAttackEntity SpecialAttack { get => _specialAttack.ThrowIfNotLoaded(); set => _specialAttack = value; }

    public string Data { get; set; } = null!;
}
