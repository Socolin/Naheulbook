using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class RandomNameGeneratorNotFound : Exception
    {
        public string Sex { get; }
        public Guid OriginId { get; }

        public RandomNameGeneratorNotFound(string sex, Guid originId)
        {
            Sex = sex;
            OriginId = originId;
        }
    }
}