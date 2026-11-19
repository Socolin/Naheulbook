namespace Naheulbook.Core.Features.Handicap;

public class HandicapAlreadyKnownException(Guid handicapId) : Exception
{
    public Guid HandicapId { get; } = handicapId;
}
