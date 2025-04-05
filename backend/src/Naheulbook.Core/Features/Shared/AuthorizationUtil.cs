using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Shared;

public interface IAuthorizationUtil
{
    Task EnsureAdminAccessAsync(NaheulbookExecutionContext executionContext);
    Task EnsureCanEditItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateEntity itemTemplate);
    void EnsureIsGroupOwner(NaheulbookExecutionContext executionContext, GroupEntity? group);
    bool IsGroupOwner(NaheulbookExecutionContext executionContext, GroupEntity group);
    void EnsureCharacterAccess(NaheulbookExecutionContext executionContext, CharacterEntity character);
    void EnsureIsCharacterOwner(NaheulbookExecutionContext executionContext, CharacterEntity character);
    void EnsureIsGroupOwnerOrMember(NaheulbookExecutionContext executionContext, GroupEntity group);
    void EnsureItemAccess(NaheulbookExecutionContext executionContext, ItemEntity item);
    void EnsureCanTakeItem(NaheulbookExecutionContext executionContext, ItemEntity item);
    void EnsureCanDeleteGroupInvite(NaheulbookExecutionContext executionContext, GroupInviteEntity groupInvite);
    void EnsureCanAcceptGroupInvite(NaheulbookExecutionContext executionContext, GroupInviteEntity groupInvite);
    void EnsureCanEditUser(NaheulbookExecutionContext executionContext, UserEntity user);
    Task EnsureCanEditMapLayerAsync(NaheulbookExecutionContext executionContext, MapLayerEntity mapLayer);
}

public class AuthorizationUtil(IUnitOfWorkFactory unitOfWorkFactory) : IAuthorizationUtil
{
    public async Task EnsureAdminAccessAsync(NaheulbookExecutionContext executionContext)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var user = await uow.Users.GetAsync(executionContext.UserId);
            if (user?.Admin != true)
                throw new ForbiddenAccessException();
        }
    }

    public async Task EnsureCanEditItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateEntity itemTemplate)
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

    public void EnsureIsGroupOwner(NaheulbookExecutionContext executionContext, GroupEntity? group)
    {
        if (group == null)
            throw new ForbiddenAccessException();
        if (group.MasterId != executionContext.UserId)
            throw new ForbiddenAccessException();
    }

    public bool IsGroupOwner(NaheulbookExecutionContext executionContext, GroupEntity group)
    {
        return group.MasterId != executionContext.UserId;
    }

    public void EnsureCharacterAccess(NaheulbookExecutionContext executionContext, CharacterEntity character)
    {
        if (character.OwnerId != executionContext.UserId
            && character.GroupId != null
            && character.Group!.MasterId != executionContext.UserId)
            throw new ForbiddenAccessException();
    }

    public void EnsureIsCharacterOwner(NaheulbookExecutionContext executionContext, CharacterEntity character)
    {
        if (character.OwnerId != executionContext.UserId)
            throw new ForbiddenAccessException();
    }

    public void EnsureIsGroupOwnerOrMember(NaheulbookExecutionContext executionContext, GroupEntity group)
    {
        if (group.MasterId == executionContext.UserId)
            return;

        if (group.Characters.Any(c => c.OwnerId == executionContext.UserId))
            return;

        throw new ForbiddenAccessException();
    }

    public void EnsureItemAccess(NaheulbookExecutionContext executionContext, ItemEntity item)
    {
        if (item.CharacterId != null)
        {
            if (item.Character!.OwnerId == executionContext.UserId)
                return;

            if (item.Character!.GroupId != null && item.Character!.Group!.MasterId == executionContext.UserId)
                return;
        }

        if (item.MonsterId != null && item.Monster!.Group.MasterId == executionContext.UserId)
            return;

        if (item.LootId != null && item.Loot!.Group.MasterId == executionContext.UserId)
            return;

        throw new ForbiddenAccessException();
    }

    public void EnsureCanTakeItem(NaheulbookExecutionContext executionContext, ItemEntity item)
    {
        if (item.MonsterId.HasValue && item.Monster!.Group.MasterId == executionContext.UserId)
            return;

        if (item.MonsterId.HasValue && item.Monster!.Group.Characters.Any(c => c.OwnerId == executionContext.UserId))
            return;

        if (item.LootId.HasValue && item.Loot!.Group.MasterId == executionContext.UserId)
            return;

        if (item.LootId.HasValue && item.Loot!.Group.Characters.Any(c => c.OwnerId == executionContext.UserId))
            return;

        throw new ForbiddenAccessException();
    }

    public void EnsureCanDeleteGroupInvite(NaheulbookExecutionContext executionContext, GroupInviteEntity groupInvite)
    {
        if (executionContext.UserId == groupInvite.Group.MasterId)
            return;

        if (executionContext.UserId == groupInvite.Character.OwnerId)
            return;

        throw new ForbiddenAccessException();
    }

    public void EnsureCanAcceptGroupInvite(NaheulbookExecutionContext executionContext, GroupInviteEntity groupInvite)
    {
        if (!groupInvite.FromGroup && executionContext.UserId == groupInvite.Group.MasterId)
            return;

        if (groupInvite.FromGroup && executionContext.UserId == groupInvite.Character.OwnerId)
            return;

        throw new ForbiddenAccessException();
    }

    public void EnsureCanEditUser(NaheulbookExecutionContext executionContext, UserEntity user)
    {
        if (executionContext.UserId == user.Id)
            return;

        throw new ForbiddenAccessException();
    }

    public async Task EnsureCanEditMapLayerAsync(NaheulbookExecutionContext executionContext, MapLayerEntity mapLayer)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        if (mapLayer.Source == "official")
        {
            var user = await uow.Users.GetAsync(executionContext.UserId);
            if (user?.Admin == true)
                return;
        }
        else
        {
            if (mapLayer.UserId == executionContext.UserId)
                return;
        }

        throw new ForbiddenAccessException();
    }
}