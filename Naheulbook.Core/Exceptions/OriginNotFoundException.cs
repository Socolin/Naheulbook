using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class OriginNotFoundException : Exception
    {
        public Guid OriginId { get; }

        public OriginNotFoundException(Guid originId)
        {
            OriginId = originId;
        }
    }
}