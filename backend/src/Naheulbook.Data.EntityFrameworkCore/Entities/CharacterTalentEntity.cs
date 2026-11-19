using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterTalentEntity
{
    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

    public Guid TalentId { get; set; }
    private TalentEntity? _talent;
    public TalentEntity Talent { get => _talent.ThrowIfNotLoaded(); set => _talent = value; }
}
