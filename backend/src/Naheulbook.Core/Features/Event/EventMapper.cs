using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Event;

public interface IEventMapper
{
    EventResponse ToResponse(EventEntity groupEvent);
}

public class EventMapper : IEventMapper
{
    public EventResponse ToResponse(EventEntity groupEvent)
    {
        return new EventResponse
        {
            Id = groupEvent.Id,
            Timestamp = groupEvent.Timestamp,
            Name = groupEvent.Name,
            Description = groupEvent.Description,
        };
    }
}
