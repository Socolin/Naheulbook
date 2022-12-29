using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class InvalidItemTemplateActionsDataException : Exception
{
    public Guid ItemTemplateId { get; }

    public InvalidItemTemplateActionsDataException(Guid itemTemplateId)
    {
        ItemTemplateId = itemTemplateId;
    }
}