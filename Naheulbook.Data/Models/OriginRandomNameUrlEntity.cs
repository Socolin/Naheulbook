using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models;

public class OriginRandomNameUrlEntity
{
    public int Id { get; set; }

    public string Sex { get; set; } = null!;
    public string Url { get; set; } = null!;

    public Guid OriginId { get; set; }
    private OriginEntity? _origin;
    public OriginEntity Origin { get => _origin.ThrowIfNotLoaded(); set => _origin = value; }
}