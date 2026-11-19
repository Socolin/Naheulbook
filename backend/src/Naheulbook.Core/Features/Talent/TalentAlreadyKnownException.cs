namespace Naheulbook.Core.Features.Talent;

public class TalentAlreadyKnownException(Guid talentId) : Exception
{
    public Guid TalentId { get; } = talentId;
}
