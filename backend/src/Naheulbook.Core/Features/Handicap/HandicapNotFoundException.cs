namespace Naheulbook.Core.Features.Handicap;

public class HandicapNotFoundException(Guid handicapId) : Exception
{
    public Guid HandicapId { get; } = handicapId;
}
