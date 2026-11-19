namespace Naheulbook.Core.Features.Mastery;

public class MasteryAlreadyKnownException(Guid masteryId) : Exception
{
    public Guid MasteryId { get; } = masteryId;
}
