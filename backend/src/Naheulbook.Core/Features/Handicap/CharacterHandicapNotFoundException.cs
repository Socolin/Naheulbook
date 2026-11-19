namespace Naheulbook.Core.Features.Handicap;

public class CharacterHandicapNotFoundException(Guid handicapId) : Exception
{
    public Guid HandicapId { get; } = handicapId;
}
