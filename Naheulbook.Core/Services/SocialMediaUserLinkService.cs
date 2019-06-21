using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface ISocialMediaUserLinkService
    {
        Task<User> GetOrCreateUserFromFacebookAsync(string name, string facebookId);
        Task AssociateUserToFacebookIdAsync(int userId, string facebookId);
        Task<User> GetOrCreateUserFromGoogleAsync(string name, string googleId);
        Task AssociateUserToGoogleIdAsync(int userId, string googleId);
    }

    public class SocialMediaUserLinkService : ISocialMediaUserLinkService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SocialMediaUserLinkService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<User> GetOrCreateUserFromFacebookAsync(string name, string facebookId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetByFacebookIdAsync(facebookId);
                if (user == null)
                {
                    user = new User
                    {
                        FbId = facebookId,
                        Admin = false,
                        DisplayName = name
                    };
                    uow.Users.Add(user);
                    await uow.CompleteAsync();
                }

                return user;
            }
        }

        public async Task AssociateUserToFacebookIdAsync(int userId, string facebookId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetAsync(userId);
                user.FbId = facebookId;
                await uow.CompleteAsync();
            }
        }

        public async Task<User> GetOrCreateUserFromGoogleAsync(string name, string googleId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetByGoogleIdAsync(googleId);
                if (user == null)
                {
                    user = new User
                    {
                        GoogleId = googleId,
                        Admin = false,
                        DisplayName = name
                    };
                    uow.Users.Add(user);
                    await uow.CompleteAsync();
                }

                return user;
            }
        }

        public async Task AssociateUserToGoogleIdAsync(int userId, string googleId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetAsync(userId);
                user.GoogleId = googleId;
                await uow.CompleteAsync();
            }
        }
    }
}