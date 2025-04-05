using System.Security.Cryptography;

namespace Naheulbook.Core.Features.Users;

public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string password);
}

public class PasswordHashingService : IPasswordHashingService
{
    public string HashPassword(string password)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 2000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(20);

        var hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var hashBytes = Convert.FromBase64String(hashedPassword);
        var salt = new byte[16];

        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 2000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(20);

        for (var i = 0; i < 20; i++)
            if (hashBytes[i + 16] != hash[i])
                return false;

        return true;
    }
}