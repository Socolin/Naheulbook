using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class UserAccessTokenEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public DateTimeOffset DateCreated { get; set; }

    public int UserId { get; set; }
    private UserEntity? _user;
    public UserEntity User { get => _user.ThrowIfNotLoaded(); set => _user = value; }
}