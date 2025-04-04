using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Features.Users;

public interface ISocialMediaUserLinkService
{
    Task<UserEntity> GetOrCreateUserFromFacebookAsync(string name, string facebookId);
    Task AssociateUserToFacebookIdAsync(int userId, string facebookId);
    Task<UserEntity> GetOrCreateUserFromGoogleAsync(string name, string googleId);
    Task AssociateUserToGoogleIdAsync(int userId, string googleId);
    Task<UserEntity> GetOrCreateUserFromTwitterAsync(string name, string twitterId);
    Task AssociateUserToTwitterIdAsync(int userId, string twitterId);
    Task<UserEntity> GetOrCreateUserFromMicrosoftAsync(string name, string microsoftId);
    Task AssociateUserToMicrosoftIdAsync(int userId, string microsoftId);
}

public class SocialMediaUserLinkService(IUnitOfWorkFactory unitOfWorkFactory) : ISocialMediaUserLinkService
{
    public async Task<UserEntity> GetOrCreateUserFromFacebookAsync(string name, string facebookId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByFacebookIdAsync(facebookId);
        if (user == null)
        {
            user = new UserEntity
            {
                FbId = facebookId,
                Admin = false,
                DisplayName = name,
            };
            uow.Users.Add(user);
            await uow.SaveChangesAsync();
        }

        return user;
    }

    public async Task AssociateUserToFacebookIdAsync(int userId, string facebookId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId);
        if (user == null)
            throw new UserNotFoundException(userId);
        user.FbId = facebookId;
        await uow.SaveChangesAsync();
    }

    public async Task<UserEntity> GetOrCreateUserFromGoogleAsync(string name, string googleId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByGoogleIdAsync(googleId);
        if (user == null)
        {
            user = new UserEntity
            {
                GoogleId = googleId,
                Admin = false,
                DisplayName = name,
            };
            uow.Users.Add(user);
            await uow.SaveChangesAsync();
        }

        return user;
    }

    public async Task AssociateUserToGoogleIdAsync(int userId, string googleId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId);
        if (user == null)
            throw new UserNotFoundException(userId);
        user.GoogleId = googleId;
        await uow.SaveChangesAsync();
    }

    public async Task<UserEntity> GetOrCreateUserFromTwitterAsync(string name, string twitterId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByTwitterIdAsync(twitterId);
        if (user == null)
        {
            user = new UserEntity
            {
                TwitterId = twitterId,
                Admin = false,
                DisplayName = name,
            };
            uow.Users.Add(user);
            await uow.SaveChangesAsync();
        }

        return user;
    }

    public async Task AssociateUserToTwitterIdAsync(int userId, string twitterId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId);
        if (user == null)
            throw new UserNotFoundException(userId);
        user.TwitterId = twitterId;
        await uow.SaveChangesAsync();
    }

    public async Task<UserEntity> GetOrCreateUserFromMicrosoftAsync(string name, string microsoftId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByMicrosoftIdAsync(microsoftId);
        if (user == null)
        {
            user = new UserEntity
            {
                MicrosoftId = microsoftId,
                Admin = false,
                DisplayName = name,
            };
            uow.Users.Add(user);
            await uow.SaveChangesAsync();
        }

        return user;
    }

    public async Task AssociateUserToMicrosoftIdAsync(int userId, string microsoftId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId);
        if (user == null)
            throw new UserNotFoundException(userId);
        user.MicrosoftId = microsoftId;
        await uow.SaveChangesAsync();
    }
}