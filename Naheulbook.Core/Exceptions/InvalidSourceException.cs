using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions
{
    public class InvalidSourceException : Exception
    {
        public string SourceValue { get; }

        public InvalidSourceException(string source)
        {
            SourceValue = source;
        }
    }
}