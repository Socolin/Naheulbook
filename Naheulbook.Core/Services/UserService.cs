using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface IUserService
    {
        Task CreateUserAsync(string username, string password);
        Task ValidateUserAsync(string username, string activationCode);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IMailService _mailService;

        public UserService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPasswordHashingService passwordHashingService,
            IMailService mailService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _passwordHashingService = passwordHashingService;
            _mailService = mailService;
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
                await uow.CompleteAsync();
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
                await uow.CompleteAsync();
            }
        }
    }
}