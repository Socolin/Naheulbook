// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Item;

public class EmptyItemTemplateSubCategoryException(int itemTemplateSubCategoryId) : Exception
{
    public int ItemTemplateSubCategoryId { get; } = itemTemplateSubCategoryId;
}