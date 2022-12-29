using System;
using System.Collections.Generic;

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
}