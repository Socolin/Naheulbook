using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IEventService
    {
        Task<List<Event>> GetEventsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task<Event> CreateEventAsync(NaheulbookExecutionContext executionContext, int groupId, CreateEventRequest request);
        Task DeleteEventAsync(NaheulbookExecutionContext executionContext, int groupId, int eventId);
    }

    public class EventService : IEventService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;

        public EventService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<List<Event>> GetEventsForGroupAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                return await uow.Events.GetByGroupIdAsync(groupId);
            }
        }

        public async Task<Event> CreateEventAsync(NaheulbookExecutionContext executionContext, int groupId, CreateEventRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                var groupEvent = new Event
                {
                    Name = request.Name,
                    Description = request.Description,
                    GroupId = groupId,
                    Timestamp = request.Timestamp
                };

                uow.Events.Add(groupEvent);

                await uow.CompleteAsync();

                return groupEvent;
            }
        }

        public async Task DeleteEventAsync(NaheulbookExecutionContext executionContext, int groupId, int eventId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var group = await uow.Groups.GetAsync(groupId);
                if (group == null)
                    throw new GroupNotFoundException(groupId);

                _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

                var groupEvent = await uow.Events.GetAsync(eventId);
                if (groupEvent.GroupId != group.Id)
                    throw new EventNotFoundException(eventId);

                uow.Events.Remove(groupEvent);

                await uow.CompleteAsync();
            }
        }
    }
}