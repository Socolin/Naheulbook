namespace Naheulbook.Core.Features.Mastery;

public class CharacterMasteryNotFoundException(Guid masteryId) : Exception
{
    public Guid MasteryId { get; } = masteryId;
}
