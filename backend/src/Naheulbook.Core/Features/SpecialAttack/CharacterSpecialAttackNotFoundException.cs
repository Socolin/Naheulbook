namespace Naheulbook.Core.Features.SpecialAttack;

public class CharacterSpecialAttackNotFoundException(Guid specialAttackId) : Exception
{
    public Guid SpecialAttackId { get; } = specialAttackId;
}
