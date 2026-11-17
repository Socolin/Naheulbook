using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Users;

public interface IUserAccessTokenService
{
    Task<IList<UserAccessTokenEntity>> GetUserAccessTokensAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserAccessTokenEntity> CreateUserAccessTokenAsync(int userId, CreateAccessTokenRequest request, CancellationToken cancellationToken = default);
    Task<UserAccessTokenEntity?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task DeleteUserAccessTokensAsync(int userId, Guid userAccessTokenId, CancellationToken cancellationToken = default);
}

public class UserAccessTokenService(IUnitOfWorkFactory unitOfWorkFactory, ITimeService timeService)
    : IUserAccessTokenService
{
    public async Task<IList<UserAccessTokenEntity>> GetUserAccessTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.UserAccessTokenRepository.GetUserAccessTokensForUser(userId, cancellationToken);
    }

    public async Task<UserAccessTokenEntity> CreateUserAccessTokenAsync(int userId, CreateAccessTokenRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var token = new UserAccessTokenEntity
        {
            Id = Guid.NewGuid(),
            Key = RngHelper.GetRandomHexString(64),
            Name = request.Name,
            UserId = userId,
            DateCreated = timeService.UtcNow,
        };
        uow.UserAccessTokenRepository.Add(token);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        return token;
    }

    public async Task<UserAccessTokenEntity?> ValidateTokenAsync(string accessKey, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var token = await uow.UserAccessTokenRepository.GetByKeyAsync(accessKey, cancellationToken);
        return token;
    }

    public async Task DeleteUserAccessTokensAsync(int userId, Guid userAccessTokenId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var token = await uow.UserAccessTokenRepository.GetByUserIdAndTokenIdAsync(userId, userAccessTokenId, cancellationToken);
        if (token == null)
            throw new UserAccessTokenNotFoundException(userId, userAccessTokenId);
        uow.UserAccessTokenRepository.Remove(token);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }
}