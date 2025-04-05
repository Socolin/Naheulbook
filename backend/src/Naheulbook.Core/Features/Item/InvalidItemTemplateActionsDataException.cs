

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Item;

public class InvalidItemTemplateActionsDataException(Guid itemTemplateId) : Exception
{
    public Guid ItemTemplateId { get; } = itemTemplateId;
}