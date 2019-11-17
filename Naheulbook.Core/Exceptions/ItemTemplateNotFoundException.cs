using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class ItemTemplateNotFoundException : Exception
    {
        public Guid ItemTemplateId { get; }

        public ItemTemplateNotFoundException(Guid itemTemplateId)
        {
            ItemTemplateId = itemTemplateId;
        }
    }
}