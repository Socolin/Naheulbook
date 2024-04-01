using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MapNotFoundException(int mapId) : Exception
{
    public int MapId { get; } = mapId;
}