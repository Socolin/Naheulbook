using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Character;

public class RandomNameGeneratorNotFoundException(string sex, Guid originId) : Exception
{
    public string Sex { get; } = sex;
    public Guid OriginId { get; } = originId;
}