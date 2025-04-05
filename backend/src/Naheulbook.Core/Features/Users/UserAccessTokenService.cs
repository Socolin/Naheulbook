using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Users;

public interface IUserAccessTokenService
{
    Task<IList<UserAccessTokenEntity>> GetUserAccessTokensAsync(int userId);
    Task<UserAccessTokenEntity> CreateUserAccessTokenAsync(int userId, CreateAccessTokenRequest request);
    Task<UserAccessTokenEntity?> ValidateTokenAsync(string token);
    Task DeleteUserAccessTokensAsync(int userId, Guid userAccessTokenId);
}

public class UserAccessTokenService(IUnitOfWorkFactory unitOfWorkFactory, ITimeService timeService)
    : IUserAccessTokenService
{
    public async Task<IList<UserAccessTokenEntity>> GetUserAccessTokensAsync(int userId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.UserAccessTokenRepository.GetUserAccessTokensForUser(userId);
    }

    public async Task<UserAccessTokenEntity> CreateUserAccessTokenAsync(int userId, CreateAccessTokenRequest request)
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
        await uow.SaveChangesAsync();
        return token;
    }

    public async Task<UserAccessTokenEntity?> ValidateTokenAsync(string accessKey)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var token = await uow.UserAccessTokenRepository.GetByKeyAsync(accessKey);
        return token;
    }

    public async Task DeleteUserAccessTokensAsync(int userId, Guid userAccessTokenId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var token = await uow.UserAccessTokenRepository.GetByUserIdAndTokenIdAsync(userId, userAccessTokenId);
        if (token == null)
            throw new UserAccessTokenNotFoundException(userId, userAccessTokenId);
        uow.UserAccessTokenRepository.Remove(token);
        await uow.SaveChangesAsync();
    }
}