using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Users;

public interface IUserService
{
    Task CreateUserAsync(string username, string password);
    Task ValidateUserAsync(string username, string activationCode);
    Task<UserEntity> CheckPasswordAsync(string username, string password);
    Task<UserEntity> GetUserInfoAsync(int userId);
    Task UpdateUserAsync(NaheulbookExecutionContext executionContext, int userId, UpdateUserRequest request);
    Task<List<UserEntity>> SearchUserAsync(NaheulbookExecutionContext executionContext, string filter);
}

public class UserService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHashingService passwordHashingService,
    IMailService mailService,
    IAuthorizationUtil authorizationUtil
) : IUserService
{
    public async Task CreateUserAsync(string username, string password)
    {
        string activationCode;

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
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
            var user = new UserEntity()
            {
                Username = username,
                HashedPassword = passwordHashingService.HashPassword(password),
                ActivationCode = activationCode,
            };

            uow.Users.Add(user);
            await uow.SaveChangesAsync();
        }

        await mailService.SendCreateUserMailAsync(username, activationCode);
    }

    public async Task ValidateUserAsync(string username, string activationCode)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByUsernameAsync(username);
        if (user == null)
            throw new UserNotFoundException();
        if (user.ActivationCode != activationCode)
            throw new InvalidUserActivationCodeException();
        user.ActivationCode = null;
        await uow.SaveChangesAsync();
    }

    public async Task<UserEntity> CheckPasswordAsync(string username, string password)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByUsernameAsync(username);
        if (user == null)
            throw new UserNotFoundException();
        if (user.HashedPassword == null)
            throw new InvalidPasswordException();
        var success = passwordHashingService.VerifyPassword(user.HashedPassword, password);
        if (!success)
            throw new InvalidPasswordException();
        return user;
    }

    public async Task<UserEntity> GetUserInfoAsync(int userId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId);
        if (user == null)
            throw new UserNotFoundException(userId);
        return user;
    }

    public async Task UpdateUserAsync(NaheulbookExecutionContext executionContext, int userId, UpdateUserRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId);
        if (user == null)
            throw new UserNotFoundException();

        authorizationUtil.EnsureCanEditUser(executionContext, user);

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

    public async Task<List<UserEntity>> SearchUserAsync(NaheulbookExecutionContext executionContext, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return new List<UserEntity>();

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Users.SearchUsersAsync(filter);
    }
}