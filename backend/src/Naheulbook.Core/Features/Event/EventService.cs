using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Event;

public interface IEventService
{
    Task<List<EventEntity>> GetEventsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default);
    Task<EventEntity> CreateEventAsync(NaheulbookExecutionContext executionContext, int groupId, CreateEventRequest request, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(NaheulbookExecutionContext executionContext, int groupId, int eventId, CancellationToken cancellationToken = default);
}

public class EventService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil
) : IEventService
{
    public async Task<List<EventEntity>> GetEventsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Events.GetByGroupIdAsync(groupId, cancellationToken);
    }

    public async Task<EventEntity> CreateEventAsync(NaheulbookExecutionContext executionContext, int groupId, CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var groupEvent = new EventEntity
        {
            Name = request.Name,
            Description = request.Description,
            GroupId = groupId,
            Timestamp = request.Timestamp,
        };

        uow.Events.Add(groupEvent);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return groupEvent;
    }

    public async Task DeleteEventAsync(NaheulbookExecutionContext executionContext, int groupId, int eventId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId, cancellationToken);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var groupEvent = await uow.Events.GetAsync(eventId, cancellationToken);
        if (groupEvent == null || groupEvent.GroupId != group.Id)
            throw new EventNotFoundException(eventId);

        uow.Events.Remove(groupEvent);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
    }
}