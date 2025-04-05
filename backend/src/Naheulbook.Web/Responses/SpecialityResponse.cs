using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class SpecialityResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<StatModifierResponse> Modifiers { get; set; } = null!;
    public List<SpecialitySpecialResponse> Specials { get; set; } = null!;
    public List<FlagResponse>? Flags { get; set; }
}

[PublicAPI]
public class SpecialitySpecialResponse
{
    public int Id { get; set; }
    public bool IsBonus { get; set; }
    public string Description { get; set; } = null!;
    public List<FlagResponse>? Flags { get; set; }
}