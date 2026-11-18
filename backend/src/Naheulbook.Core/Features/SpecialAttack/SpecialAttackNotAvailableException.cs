namespace Naheulbook.Core.Features.SpecialAttack;

public class SpecialAttackNotAvailableException(Guid specialAttackId) : Exception
{
    public Guid SpecialAttackId { get; } = specialAttackId;
}
