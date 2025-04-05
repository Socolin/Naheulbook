using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? HashedPassword { get; set; }

    public string? ActivationCode { get; set; }

    public bool Admin { get; set; }

    public string? FbId { get; set; }

    public string? GoogleId { get; set; }

    public string? MicrosoftId { get; set; }

    public string? TwitterId { get; set; }

    public string? DisplayName { get; set; }

    public DateTime? ShowInSearchUntil { get; set; }

    private ICollection<GroupEntity>? _groups;
    public ICollection<GroupEntity> Groups { get => _groups.ThrowIfNotLoaded(); set => _groups = value; }

    private ICollection<CharacterEntity>? _characters;
    public ICollection<CharacterEntity> Characters { get => _characters.ThrowIfNotLoaded(); set => _characters = value; }
}