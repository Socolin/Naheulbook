using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class OriginNotFoundException : Exception
    {
        public int OriginId { get; }

        public OriginNotFoundException(int originId)
        {
            OriginId = originId;
        }
    }
}