using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}