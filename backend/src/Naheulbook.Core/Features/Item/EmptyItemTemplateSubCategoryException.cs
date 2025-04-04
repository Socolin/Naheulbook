// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Naheulbook.Core.Features.Item;

public class EmptyItemTemplateSubCategoryException(int itemTemplateSubCategoryId) : Exception
{
    public int ItemTemplateSubCategoryId { get; } = itemTemplateSubCategoryId;
}