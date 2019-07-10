using System.Linq;
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
        bool IsGroupOwner(NaheulbookExecutionContext executionContext, Group group);
        void EnsureCharacterAccess(NaheulbookExecutionContext executionContext, Character character);
        void EnsureIsCharacterOwner(NaheulbookExecutionContext executionContext, Character character);
        void EnsureIsGroupOwnerOrMember(NaheulbookExecutionContext executionContext, Group group);
        void EnsureItemAccess(NaheulbookExecutionContext executionContext, Item item);
        void EnsureCanDeleteGroupInvite(NaheulbookExecutionContext executionContext, GroupInvite groupInvite);
        void EnsureCanAcceptGroupInvite(NaheulbookExecutionContext executionContext, GroupInvite groupInvite);
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

        public bool IsGroupOwner(NaheulbookExecutionContext executionContext, Group group)
        {
            return group.MasterId != executionContext.UserId;
        }

        public void EnsureCharacterAccess(NaheulbookExecutionContext executionContext, Character character)
        {
            if (character.OwnerId != executionContext.UserId
                && character.GroupId != null
                && character.Group.MasterId != executionContext.UserId)
                throw new ForbiddenAccessException();
        }

        public void EnsureIsCharacterOwner(NaheulbookExecutionContext executionContext, Character character)
        {
            if (character.OwnerId != executionContext.UserId)
                throw new ForbiddenAccessException();
        }

        public void EnsureIsGroupOwnerOrMember(NaheulbookExecutionContext executionContext, Group group)
        {
            if (group.MasterId == executionContext.UserId)
                return;

            if (group.Characters.Any(c => c.OwnerId == executionContext.UserId))
                return;

            throw new ForbiddenAccessException();
        }

        public void EnsureItemAccess(NaheulbookExecutionContext executionContext, Item item)
        {
            if (item.CharacterId != null)
            {
                if (item.Character.OwnerId == executionContext.UserId)
                    return;

                if (item.Character.GroupId != null && item.Character.Group.MasterId == executionContext.UserId)
                    return;
            }

            if (item.MonsterId != null && item.Monster.Group.MasterId == executionContext.UserId)
                return;

            if (item.LootId != null && item.Loot.Group.MasterId == executionContext.UserId)
                return;

            throw new ForbiddenAccessException();
        }

        public void EnsureCanDeleteGroupInvite(NaheulbookExecutionContext executionContext, GroupInvite groupInvite)
        {
            if (executionContext.UserId == groupInvite.Group.MasterId)
                return;

            if (executionContext.UserId == groupInvite.Character.OwnerId)
                return;

            throw new ForbiddenAccessException();
        }

        public void EnsureCanAcceptGroupInvite(NaheulbookExecutionContext executionContext, GroupInvite groupInvite)
        {
            if (!groupInvite.FromGroup && executionContext.UserId == groupInvite.Group.MasterId)
                return;

            if (groupInvite.FromGroup && executionContext.UserId == groupInvite.Character.OwnerId)
                return;

            throw new ForbiddenAccessException();
        }
    }
}