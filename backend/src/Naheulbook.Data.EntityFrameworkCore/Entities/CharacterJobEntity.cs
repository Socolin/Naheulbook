using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class CharacterJobEntity
{
    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

    public Guid JobId { get; set; }
    private JobEntity? _job;
    public JobEntity Job { get => _job.ThrowIfNotLoaded(); set => _job = value; }

    public int Order { get; set; }
}