using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Entropy;

public record EntropyTables(
    List<MankdebolCalamityEntity> MankdebolCalamities,
    List<DebilibeukBlessingEntity> DebilibeukBlessings
);

public interface IEntropyService
{
    Task<EntropyTables> GetEntropyTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Apply the calamity / blessing chosen by the GM: consume the group points and log it in the group history (GM only)
    /// </summary>
    Task ApplyEntropyEventAsync(
        NaheulbookExecutionContext executionContext,
        int groupId,
        EntropyKind kind,
        PostApplyEntropyEventRequest request,
        CancellationToken cancellationToken = default
    );
}

public class EntropyService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    INotificationSessionFactory notificationSessionFactory,
    IGroupUtil groupUtil
) : IEntropyService
{
    public async Task<EntropyTables> GetEntropyTablesAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var calamities = await uow.MankdebolCalamities.GetAllOrderedByDiceRollAsync(cancellationToken);
        var blessings = await uow.DebilibeukBlessings.GetAllOrderedByDiceRollAsync(cancellationToken);
        return new EntropyTables(calamities, blessings);
    }

    public async Task ApplyEntropyEventAsync(
        NaheulbookExecutionContext executionContext,
        int groupId,
        EntropyKind kind,
        PostApplyEntropyEventRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var group = await uow.Groups.GetGroupsWithCharactersAsync(groupId, cancellationToken);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            IEntropyEventEntity? entropyEvent = kind switch
            {
                EntropyKind.MankdebolCalamity => await uow.MankdebolCalamities.GetAsync(request.EventId, cancellationToken),
                EntropyKind.DebilibeukBlessing => await uow.DebilibeukBlessings.GetAsync(request.EventId, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
            };
            if (entropyEvent == null)
                throw new EntropyEventNotFoundException(kind, request.EventId);

            CharacterEntity? character = null;
            if (request.CharacterId.HasValue)
            {
                character = group.Characters.FirstOrDefault(c => c.Id == request.CharacterId.Value);
                if (character == null)
                    throw new CharacterNotInGroupException(groupId, request.CharacterId.Value);
            }

            groupUtil.ApplyEntropyEventAndNotify(group, kind, entropyEvent, character, notificationSession);

            await uow.SaveChangesAsync(DoNotInterrupt.Token);
        }

        await notificationSession.CommitAsync();
    }
}
