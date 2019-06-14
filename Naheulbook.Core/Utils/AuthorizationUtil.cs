using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Utils
{
    public interface IAuthorizationUtil
    {
        Task EnsureAdminAccessAsync(NaheulbookExecutionContext executionContext);
        Task EnsureCanEditItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplate itemTemplate);
        void EnsureIsGroupOwner(NaheulbookExecutionContext executionContext, Group group);
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

        public async Task EnsureCanEditItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplate itemTemplate)
        {
            switch (itemTemplate.Source)
            {
                case "official":
                    await EnsureAdminAccessAsync(executionContext);
                    break;
                case "private":
                case "community":
                    if (itemTemplate.SourceUserId != executionContext.UserId)
                        throw new ForbiddenAccessException();
                    break;
                default:
                    throw new ForbiddenAccessException();
            }
        }

        public void EnsureIsGroupOwner(NaheulbookExecutionContext executionContext, Group group)
        {
            if (group.MasterId != executionContext.UserId)
                throw new ForbiddenAccessException();
        }
    }
}