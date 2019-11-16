using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Requests.Requests
{
    public class CharacterLevelUpRequest
    {
        public string EvOrEa { get; set; } = null!;
        public short EvOrEaValue { get; set; }
        public int TargetLevelUp { get; set; }
        public string StatToUp { get; set; } = null!;
        public Guid? SkillId { get; set; }
        public List<int> SpecialityIds { get; set; } = null!;
    }
}