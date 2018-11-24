using System.Collections.Generic;

namespace Naheulbook.Web.Responses
{
    public class SpecialityResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<StatModifierResponse> Modifiers { get; set; }
        public List<SpecialitySpecialResponse> Specials { get; set; }
        public List<FlagResponse> Flags { get; set; }
    }

    public class SpecialitySpecialResponse
    {
        public int Id { get; set; }
        public bool IsBonus { get; set; }
        public string Description { get; set; }
        public List<FlagResponse> Flags { get; set; }
    }
}