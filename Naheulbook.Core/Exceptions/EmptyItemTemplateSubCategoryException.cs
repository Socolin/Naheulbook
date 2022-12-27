// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Naheulbook.Core.Exceptions;

public class EmptyItemTemplateSubCategoryException : Exception
{
    public int ItemTemplateSubCategoryId { get; }

    public EmptyItemTemplateSubCategoryException(int itemTemplateSubCategoryId)
    {
        ItemTemplateSubCategoryId = itemTemplateSubCategoryId;
    }
}