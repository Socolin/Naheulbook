using System;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Shared.TransientModels
{
    public class NhbkAction
    {
        public string Type { get; set; } = null!;
        public bool Hidden { get; set; }
        public NhbkActionData? Data { get; set; }
    }

    public class NhbkActionData
    {
        public Guid? TemplateId { get; set; }
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public int? Ev { get; set; }
        public int? Ea { get; set; }
        public int? EffectId { get; set; }
        public ActiveStatsModifier? Modifier { get; set; }
        public string? Text { get; set; }
        public JObject? EffectData { get; set; }
    }
}