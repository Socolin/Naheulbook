using System.Security.Cryptography;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Users;

public interface IUserService
{
    Task CreateUserAsync(string username, string password, CancellationToken cancellationToken = default);
    Task ValidateUserAsync(string username, string activationCode, CancellationToken cancellationToken = default);
    Task<UserEntity> CheckPasswordAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<UserEntity> GetUserInfoAsync(int userId, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(NaheulbookExecutionContext executionContext, int userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<List<UserEntity>> SearchUserAsync(NaheulbookExecutionContext executionContext, string filter, CancellationToken cancellationToken = default);
    Task<List<UserEntity>> SearchUserByIdentifiersAsync(NaheulbookExecutionContext executionContext, SearchUserByIdentifiersRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(NaheulbookExecutionContext executionContext, int userId, CancellationToken cancellationToken = default);
}

public class UserService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IPasswordHashingService passwordHashingService,
    IMailService mailService,
    IAuthorizationUtil authorizationUtil
) : IUserService
{
    public async Task CreateUserAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        string activationCode;

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var alreadyExistingUser = await uow.Users.GetByUsernameAsync(username, cancellationToken);
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
            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        await mailService.SendCreateUserMailAsync(username, activationCode, cancellationToken);
    }

    public async Task ValidateUserAsync(string username, string activationCode, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByUsernameAsync(username, cancellationToken);
        if (user == null)
            throw new UserNotFoundException();
        if (user.ActivationCode != activationCode)
            throw new InvalidUserActivationCodeException();
        user.ActivationCode = null;
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }

    public async Task<UserEntity> CheckPasswordAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetByUsernameAsync(username, cancellationToken);
        if (user == null)
            throw new UserNotFoundException();
        if (user.HashedPassword == null)
            throw new InvalidPasswordException();
        var success = passwordHashingService.VerifyPassword(user.HashedPassword, password);
        if (!success)
            throw new InvalidPasswordException();
        return user;
    }

    public async Task<UserEntity> GetUserInfoAsync(int userId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId, cancellationToken);
        if (user == null)
            throw new UserNotFoundException(userId);
        return user;
    }

    public async Task UpdateUserAsync(NaheulbookExecutionContext executionContext, int userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId, cancellationToken);
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


        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }

    public async Task<List<UserEntity>> SearchUserAsync(NaheulbookExecutionContext executionContext, string filter, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return new List<UserEntity>();

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Users.SearchUsersAsync(filter, cancellationToken);
    }

    public async Task<List<UserEntity>> SearchUserByIdentifiersAsync(
        NaheulbookExecutionContext executionContext,
        SearchUserByIdentifiersRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await authorizationUtil.EnsureSuperAdminAccessAsync(executionContext, cancellationToken);

        var filter = new UserIdentifiersFilter(
            NullIfBlank(request.Username),
            NullIfBlank(request.FacebookId),
            NullIfBlank(request.GoogleId),
            NullIfBlank(request.MicrosoftId),
            NullIfBlank(request.TwitterId)
        );
        if (filter.IsEmpty)
            return [];

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Users.SearchByIdentifiersAsync(filter, cancellationToken);
    }

    public async Task DeleteUserAsync(NaheulbookExecutionContext executionContext, int userId, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureSuperAdminAccessAsync(executionContext, cancellationToken);

        if (executionContext.UserId == userId)
            throw new CannotDeleteOwnUserException();

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var user = await uow.Users.GetAsync(userId, cancellationToken);
        if (user == null)
            throw new UserNotFoundException(userId);

        await uow.Users.DeleteWithOwnedDataAsync(userId, DoNotInterrupt.Token);
    }

    private static string? NullIfBlank(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
