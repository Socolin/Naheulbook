using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.Shared.TransientModels;

/// <summary>Zgarug teeth, the currency used in the Confins du Givre. Serialized as <c>pz</c>, <c>mz</c> and <c>gz</c></summary>
[JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
public enum ZgarugCurrency
{
    /// <summary>Petite dent zgarug (PZ)</summary>
    Pz,

    /// <summary>Dent moyenne zgarug (MZ)</summary>
    Mz,

    /// <summary>Grosse dent zgarug (GZ)</summary>
    Gz,
}
