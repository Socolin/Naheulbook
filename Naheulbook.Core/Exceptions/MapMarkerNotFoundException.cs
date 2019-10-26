using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class MapMarkerNotFoundException : Exception
    {
        public int MapMarkerId { get; }

        public MapMarkerNotFoundException(int mapId)
        {
            MapMarkerId = mapId;
        }
    }
}