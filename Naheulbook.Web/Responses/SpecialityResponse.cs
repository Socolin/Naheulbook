using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses
{
    public class SpecialityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<StatModifierResponse> Modifiers { get; set; } = null!;
        public List<SpecialitySpecialResponse> Specials { get; set; } = null!;
        public List<FlagResponse>? Flags { get; set; }
    }

    public class SpecialitySpecialResponse
    {
        public int Id { get; set; }
        public bool IsBonus { get; set; }
        public string Description { get; set; } = null!;
        public List<FlagResponse>? Flags { get; set; }
    }
}