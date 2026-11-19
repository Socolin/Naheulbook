namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CharacterLevelUpRequest
{
    public required string EvOrEa { get; set; }
    public short EvOrEaValue { get; set; }
    public int TargetLevelUp { get; set; }
    public required string StatToUp { get; set; }
    public Guid? SkillId { get; set; }
    public required  List<Guid> SpecialityIds { get; set; }

    /// <summary>
    /// Talent learned with the talent or mastery point of odd levels (3, 5, 7...). Exclusive with <see cref="MasteryId"/>.
    /// </summary>
    public Guid? TalentId { get; set; }

    /// <summary>
    /// Mastery learned with the talent or mastery point of odd levels (3, 5, 7...). Exclusive with <see cref="TalentId"/>.
    /// </summary>
    public Guid? MasteryId { get; set; }
}