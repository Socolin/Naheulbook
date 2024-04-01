using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class RandomNameGeneratorNotFound(string sex, Guid originId) : Exception
{
    public string Sex { get; } = sex;
    public Guid OriginId { get; } = originId;
}