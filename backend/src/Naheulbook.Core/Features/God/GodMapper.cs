using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.God;

public interface IGodMapper
{
    GodResponse ToResponse(GodEntity god);
}

public class GodMapper : IGodMapper
{
    public GodResponse ToResponse(GodEntity god)
    {
        return new GodResponse
        {
            Id = god.Id,
            DisplayName = god.DisplayName,
            Description = god.Description,
            TechName = god.TechName,
        };
    }
}
