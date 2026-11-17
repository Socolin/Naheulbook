using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Stat;

public interface IStatMapper
{
    StatResponse ToResponse(StatEntity stat);
}

public class StatMapper : IStatMapper
{
    public StatResponse ToResponse(StatEntity stat)
    {
        return new StatResponse
        {
            Name = stat.Name,
            DisplayName = stat.DisplayName,
            Description = stat.Description,
        };
    }
}
