using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Users;

public interface ISocialMediaUserLinkService
{
    Task<UserEntity> GetOrCreateUserFromFacebookAsync(string name, string facebookId, CancellationToken cancellationToken = default);
    Task AssociateUserToFacebookIdAsync(int userId, string facebookId, CancellationToken cancellationToken = default);
    Task<UserEntity> GetOrCreateUserFromGoogleAsync(string name, string googleId, CancellationToken cancellationToken = default);
    Task AssociateUserToGoogleIdAsync(int userId, string googleId, CancellationToken cancellationToken = default);
    Task<UserEntity> GetOrCreateUserFromMicrosoftAsync(string name, string microsoftId, CancellationToken cancellationToken = default);
    Task AssociateUserToMicrosoftIdAsync(int userId, string microsoftId, CancellationToken cancellationToken = default);
}

public class SocialMediaUserLinkService(IUnitOfWorkFactory unitOfWorkFactory) : ISocialMediaUserLinkService
{
    public async Task<UserEntity> GetOrCreateUserFromFacebookAsync(string name, string facebookId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByFacebookIdAsync(facebookId, cancellationToken);
        if (user == null)
        {
            user = new UserEntity
            {
                FbId = facebookId,
                AdminLevel = UserAdminLevel.None,
                DisplayName = name,
            };
            uow.Users.Add(user);
            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        return user;
    }

    public async Task AssociateUserToFacebookIdAsync(int userId, string facebookId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId, cancellationToken);
        if (user == null)
            throw new UserNotFoundException(userId);
        user.FbId = facebookId;
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }

    public async Task<UserEntity> GetOrCreateUserFromGoogleAsync(string name, string googleId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByGoogleIdAsync(googleId, cancellationToken);
        if (user == null)
        {
            user = new UserEntity
            {
                GoogleId = googleId,
                AdminLevel = UserAdminLevel.None,
                DisplayName = name,
            };
            uow.Users.Add(user);
            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        return user;
    }

    public async Task AssociateUserToGoogleIdAsync(int userId, string googleId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId, cancellationToken);
        if (user == null)
            throw new UserNotFoundException(userId);
        user.GoogleId = googleId;
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }

    public async Task<UserEntity> GetOrCreateUserFromMicrosoftAsync(string name, string microsoftId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByMicrosoftIdAsync(microsoftId, cancellationToken);
        if (user == null)
        {
            user = new UserEntity
            {
                MicrosoftId = microsoftId,
                AdminLevel = UserAdminLevel.None,
                DisplayName = name,
            };
            uow.Users.Add(user);
            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        return user;
    }

    public async Task AssociateUserToMicrosoftIdAsync(int userId, string microsoftId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId, cancellationToken);
        if (user == null)
            throw new UserNotFoundException(userId);
        user.MicrosoftId = microsoftId;
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }
}