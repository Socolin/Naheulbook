using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions
{
    public class MonsterSubCategoryNotFoundException : Exception
    {
        public int SubCategoryId { get; }

        public MonsterSubCategoryNotFoundException(int subCategoryId)
        {
            SubCategoryId = subCategoryId;
        }
    }
}