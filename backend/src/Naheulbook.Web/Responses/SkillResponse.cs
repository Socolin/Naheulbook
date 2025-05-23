using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class SkillResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? PlayerDescription { get; set; }
    public string? Require { get; set; }
    public string? Resist { get; set; }
    public string? Using { get; set; }
    public string? Roleplay { get; set; }
    public string[] Stat { get; set; } = null!;
    public short? Test { get; set; }
    public List<FlagResponse>? Flags { get; set; }
    public List<SkillEffectResponse> Effects { get; set; } = null!;
}

[PublicAPI]
public class SkillEffectResponse
{
    public string Stat { get; set; } = null!;
    public int Value { get; set; }
    public string Type { get; set; } = null!;
}