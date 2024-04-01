using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class InvalidItemTemplateActionsDataException(Guid itemTemplateId) : Exception
{
    public Guid ItemTemplateId { get; } = itemTemplateId;
}