using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions;

public class MonsterSubCategoryNotFoundException(int subCategoryId) : Exception
{
    public int SubCategoryId { get; } = subCategoryId;
}