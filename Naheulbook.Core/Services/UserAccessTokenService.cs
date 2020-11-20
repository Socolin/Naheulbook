using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Services
{
    public interface IUserAccessTokenService
    {
        Task<IList<UserAccessToken>> GetUserAccessTokensAsync(int userId);
        Task<UserAccessToken> CreateUserAccessTokenAsync(int userId, CreateAccessTokenRequest request);
        Task<UserAccessToken?> ValidateTokenAsync(string token);
        Task DeleteUserAccessTokensAsync(int userId, Guid userAccessTokenId);
    }

    public class UserAccessTokenService : IUserAccessTokenService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITimeService _timeService;

        public UserAccessTokenService(IUnitOfWorkFactory unitOfWorkFactory, ITimeService timeService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _timeService = timeService;
        }

        public async Task<IList<UserAccessToken>> GetUserAccessTokensAsync(int userId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.UserAccessTokenRepository.GetUserAccessTokensForUser(userId);
            }
        }

        public async Task<UserAccessToken> CreateUserAccessTokenAsync(int userId, CreateAccessTokenRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var token = new UserAccessToken
                {
                    Id = Guid.NewGuid(),
                    Key = RngHelper.GetRandomHexString(64),
                    Name = request.Name,
                    UserId = userId,
                    DateCreated = _timeService.UtcNow
                };
                uow.UserAccessTokenRepository.Add(token);
                await uow.SaveChangesAsync();
                return token;
            }
        }

        public async Task<UserAccessToken?> ValidateTokenAsync(string accessKey)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var token = await uow.UserAccessTokenRepository.GetByKeyAsync(accessKey);
                return token;
            }
        }

        public async Task DeleteUserAccessTokensAsync(int userId, Guid userAccessTokenId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var token = await uow.UserAccessTokenRepository.GetByUserIdAndTokenIdAsync(userId, userAccessTokenId);
                if (token == null)
                    throw new UserAccessTokenNotFoundException(userId, userAccessTokenId);
                uow.UserAccessTokenRepository.Remove(token);
                await uow.SaveChangesAsync();
            }
        }
    }
}