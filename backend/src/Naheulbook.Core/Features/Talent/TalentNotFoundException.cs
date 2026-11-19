namespace Naheulbook.Core.Features.Talent;

public class TalentNotFoundException(Guid talentId) : Exception
{
    public Guid TalentId { get; } = talentId;
}
