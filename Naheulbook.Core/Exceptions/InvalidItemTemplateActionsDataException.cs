using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class InvalidItemTemplateActionsDataException : Exception
    {
        public InvalidItemTemplateActionsDataException(int itemTemplateId)
        {
            ItemTemplateId = itemTemplateId;
        }

        public int ItemTemplateId { get; }
    }
}