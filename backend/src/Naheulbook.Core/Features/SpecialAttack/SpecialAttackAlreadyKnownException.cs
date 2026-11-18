namespace Naheulbook.Core.Features.SpecialAttack;

public class SpecialAttackAlreadyKnownException(Guid specialAttackId) : Exception
{
    public Guid SpecialAttackId { get; } = specialAttackId;
}
