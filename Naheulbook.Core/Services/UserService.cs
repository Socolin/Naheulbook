using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(string username, string password);
        Task ValidateUserAsync(string username, string activationCode);
        Task<User> CheckPasswordAsync(string username, string password);
        Task<User> GetUserInfoAsync(int userId);
        Task UpdateUserAsync(NaheulbookExecutionContext executionContext, int userId, UpdateUserRequest request);
        Task<List<User>> SearchUserAsync(NaheulbookExecutionContext executionContext, string filter);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IMailService _mailService;
        private readonly IAuthorizationUtil _authorizationUtil;

        public UserService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPasswordHashingService passwordHashingService,
            IMailService mailService,
            IAuthorizationUtil authorizationUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _passwordHashingService = passwordHashingService;
            _mailService = mailService;
            _authorizationUtil = authorizationUtil;
        }

        public async Task CreateUserAsync(string username, string password)
        {
            string activationCode;

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var alreadyExistingUser = await uow.Users.GetByUsernameAsync(username);
                if (alreadyExistingUser != null)
                    throw new UserAlreadyExistsException();

                var bytes = new byte[64];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }

                activationCode = string.Join("", bytes.Select(b => b.ToString("x2")));
                var user = new User()
                {
                    Username = username,
                    HashedPassword = _passwordHashingService.HashPassword(password),
                    ActivationCode = activationCode
                };

                uow.Users.Add(user);
                await uow.SaveChangesAsync();
            }

            await _mailService.SendCreateUserMailAsync(username, activationCode);
        }

        public async Task ValidateUserAsync(string username, string activationCode)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetByUsernameAsync(username);
                if (user == null)
                    throw new UserNotFoundException();
                if (user.ActivationCode != activationCode)
                    throw new InvalidUserActivationCodeException();
                user.ActivationCode = null;
                await uow.SaveChangesAsync();
            }
        }

        public async Task<User> CheckPasswordAsync(string username, string password)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetByUsernameAsync(username);
                if (user == null)
                    throw new UserNotFoundException();
                if (user.HashedPassword == null)
                    throw new InvalidPasswordException();
                var success = _passwordHashingService.VerifyPassword(user.HashedPassword, password);
                if (!success)
                    throw new InvalidPasswordException();
                return user;
            }
        }

        public async Task<User> GetUserInfoAsync(int userId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetAsync(userId);
                if (user == null)
                    throw new UserNotFoundException(userId);
                return user;
            }
        }

        public async Task UpdateUserAsync(NaheulbookExecutionContext executionContext, int userId, UpdateUserRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetAsync(userId);
                if (user == null)
                    throw new UserNotFoundException();

                _authorizationUtil.EnsureCanEditUser(executionContext, user);

                if (!string.IsNullOrEmpty(request.DisplayName))
                    user.DisplayName = request.DisplayName;
                if (request.ShowInSearchFor.HasValue)
                {
                    if (request.ShowInSearchFor.Value == 0)
                        user.ShowInSearchUntil = null;
                    else
                        user.ShowInSearchUntil = DateTime.UtcNow.AddSeconds(request.ShowInSearchFor.Value);
                }


                await uow.SaveChangesAsync();
            }
        }

        public async Task<List<User>> SearchUserAsync(NaheulbookExecutionContext executionContext, string filter)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Users.SearchUsersAsync(filter);
            }
        }
    }
}