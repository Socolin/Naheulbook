using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Data.Factories;

namespace Naheulbook.Core.Utils
{
    public interface IAuthorizationUtil
    {
        Task EnsureAdminAccessAsync(NaheulbookExecutionContext executionContext);
    }

    public class AuthorizationUtil : IAuthorizationUtil
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public AuthorizationUtil(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task EnsureAdminAccessAsync(NaheulbookExecutionContext executionContext)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var user = await uow.Users.GetAsync(executionContext.UserId);
                if (user?.Admin != true)
                    throw new ForbiddenAccessException();
            }
        }
    }
}