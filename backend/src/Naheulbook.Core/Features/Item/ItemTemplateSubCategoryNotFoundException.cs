using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Item;

public class ItemTemplateSubCategoryNotFoundException : Exception
{
    public int ItemTemplateSubCategoryId { get; }
    public string? ItemTemplateSubCategoryTechName { get; }

    public ItemTemplateSubCategoryNotFoundException(int itemTemplateSubCategoryId)
    {
        ItemTemplateSubCategoryId = itemTemplateSubCategoryId;
    }

    public ItemTemplateSubCategoryNotFoundException(string itemTemplateSubCategoryTechName)
    {
        ItemTemplateSubCategoryTechName = itemTemplateSubCategoryTechName;
    }
}