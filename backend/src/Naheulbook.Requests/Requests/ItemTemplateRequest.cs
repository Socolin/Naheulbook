using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class ItemTemplateRequest
{
    public required string Source { get; set; }
    public int SubCategoryId { get; set; }

    [StringLength(256, MinimumLength = 1)]
    public required string Name { get; set; }

    public string? TechName { get; set; }
    public List<ItemTemplateModifierRequest> Modifiers { get; set; } = new();
    public List<Guid> SkillIds { get; set; } = new();
    public List<Guid> UnSkillIds { get; set; } = new();
    public List<ItemTemplateSkillModifierRequest> SkillModifiers { get; set; } = new();
    public List<ItemTemplateRequirementRequest> Requirements { get; set; } = new();
    public List<IdRequest> Slots { get; set; } = new();
    public required JObject Data { get; set; }
}

[PublicAPI]
public class ItemTemplateModifierRequest
{
    public required string Stat { get; set; }
    public int Value { get; set; }
    public required string Type { get; set; }
    public List<string>? Special { get; set; }
    public Guid? JobId { get; set; }
    public Guid? OriginId { get; set; }
}

[PublicAPI]
public class ItemTemplateSkillModifierRequest
{
    public Guid SkillId { get; set; }
    public short Value { get; set; }
}

[PublicAPI]
public class ItemTemplateRequirementRequest
{
    public required string Stat { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
}