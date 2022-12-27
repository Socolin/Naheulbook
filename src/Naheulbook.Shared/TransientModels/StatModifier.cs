using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Shared.TransientModels;

public class StatModifier
{
    public string Stat { get; set; } = null!;
    public string Type { get; set; } = null!;
    public short Value { get; set; }
    public IList<string>? Special { get; set; }
}