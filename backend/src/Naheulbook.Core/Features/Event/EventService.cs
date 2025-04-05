using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Event;

public interface IEventService
{
    Task<List<EventEntity>> GetEventsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
    Task<EventEntity> CreateEventAsync(NaheulbookExecutionContext executionContext, int groupId, CreateEventRequest request);
    Task DeleteEventAsync(NaheulbookExecutionContext executionContext, int groupId, int eventId);
}

public class EventService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil
) : IEventService
{
    public async Task<List<EventEntity>> GetEventsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        return await uow.Events.GetByGroupIdAsync(groupId);
    }

    public async Task<EventEntity> CreateEventAsync(NaheulbookExecutionContext executionContext, int groupId, CreateEventRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
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

        await uow.SaveChangesAsync();

        return groupEvent;
    }

    public async Task DeleteEventAsync(NaheulbookExecutionContext executionContext, int groupId, int eventId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
        if (group == null)
            throw new GroupNotFoundException(groupId);

        authorizationUtil.EnsureIsGroupOwner(executionContext, group);

        var groupEvent = await uow.Events.GetAsync(eventId);
        if (groupEvent == null || groupEvent.GroupId != group.Id)
            throw new EventNotFoundException(eventId);

        uow.Events.Remove(groupEvent);

        await uow.SaveChangesAsync();
    }
}