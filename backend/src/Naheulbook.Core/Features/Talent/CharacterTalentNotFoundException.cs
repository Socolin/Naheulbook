namespace Naheulbook.Core.Features.Talent;

public class CharacterTalentNotFoundException(Guid talentId) : Exception
{
    public Guid TalentId { get; } = talentId;
}
