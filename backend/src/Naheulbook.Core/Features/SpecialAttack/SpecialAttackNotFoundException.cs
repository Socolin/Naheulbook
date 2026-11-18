namespace Naheulbook.Core.Features.SpecialAttack;

public class SpecialAttackNotFoundException(Guid specialAttackId) : Exception
{
    public Guid SpecialAttackId { get; } = specialAttackId;
}
