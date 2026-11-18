using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class CharacterHistoryEntryEntity : IHistoryEntry
{
    public int Id { get; set; }
    public string Action { get; set; } = null!;
    public string? Data { get; set; }
    #warning convert to date time offset + utc
    public DateTime Date { get; set; }
    public bool Gm { get; set; }

    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }
}