// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Naheulbook.Core.Exceptions;

public class EmptyItemTemplateSubCategoryException(int itemTemplateSubCategoryId) : Exception
{
    public int ItemTemplateSubCategoryId { get; } = itemTemplateSubCategoryId;
}