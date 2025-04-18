using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class ItemTemplateResponse
{
    public Guid Id { get; set; }
    public int SubCategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? TechName { get; set; }
    public string Source { get; set; } = null!;
    public int? SourceUserId { get; set; }
    public string? SourceUser { get; set; }
    public JObject Data { get; set; } = null!;

    public List<ItemTemplateModifierResponse> Modifiers { get; set; } = null!;
    public List<Guid> SkillIds { get; set; } = null!;
    public List<Guid> UnSkillIds { get; set; } = null!;

    public List<ItemTemplateSkillModifierResponse> SkillModifiers { get; set; } = null!;
    public List<ItemTemplateRequirementResponse> Requirements { get; set; } = null!;
    public List<ItemSlotResponse> Slots { get; set; } = null!;
}

public class ItemTemplateModifierResponse
{
    public string Stat { get; set; } = null!;
    public int Value { get; set; }
    public string Type { get; set; } = null!;
    public List<string>? Special { get; set; }
    public Guid? JobId { get; set; }
    public Guid? OriginId { get; set; }
}

public class ItemTemplateSkillModifierResponse
{
    public Guid SkillId { get; set; }

    public short Value { get; set; }
}

public class ItemTemplateRequirementResponse
{
    public string Stat { get; set; } = null!;
    public int Min { get; set; }
    public int Max { get; set; }
}