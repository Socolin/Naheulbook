using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class CharacterHistoryEntryEntity : IHistoryEntry
{
    public int Id { get; set; }
    public string Action { get; set; } = null!;
    public string? Data { get; set; }
    public DateTime Date { get; set; }
    public bool Gm { get; set; }
    public string? Info { get; set; }

    public int? EffectId { get; set; }
    private EffectEntity? _effect;
    public EffectEntity? Effect { get => _effect.ThrowIfNotLoadedAndNotNull(EffectId); set => _effect = value; }

    public int? CharacterModifierId { get; set; }
    private CharacterModifierEntity? _characterModifier;
    public CharacterModifierEntity? CharacterModifier { get => _characterModifier.ThrowIfNotLoadedAndNotNull(CharacterModifierId); set => _characterModifier = value; }

    public int? ItemId { get; set; }
    private ItemEntity? _item;
    public ItemEntity? Item { get => _item.ThrowIfNotLoadedAndNotNull(ItemId); set => _item = value; }

    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }
}