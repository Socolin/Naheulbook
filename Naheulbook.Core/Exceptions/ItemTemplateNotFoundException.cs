using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class ItemTemplateNotFoundException : Exception
    {
        public int ItemTemplateId { get; }

        public ItemTemplateNotFoundException(int itemTemplateId)
        {
            ItemTemplateId = itemTemplateId;
        }
    }
}