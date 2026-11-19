namespace Naheulbook.Core.Features.Mastery;

public class MasteryNotFoundException(Guid masteryId) : Exception
{
    public Guid MasteryId { get; } = masteryId;
}
