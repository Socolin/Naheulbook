using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions;

public class InvalidSourceException(string source) : Exception
{
    public string SourceValue { get; } = source;
}