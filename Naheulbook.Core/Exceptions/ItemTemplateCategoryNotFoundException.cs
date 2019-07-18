using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class ItemTemplateCategoryNotFoundException : Exception
    {
        public int ItemTemplateCategoryId { get; }
        public string ItemTemplateCategoryTechName { get; }

        public ItemTemplateCategoryNotFoundException(int itemTemplateCategoryId)
        {
            ItemTemplateCategoryId = itemTemplateCategoryId;
        }

        public ItemTemplateCategoryNotFoundException(string itemTemplateCategoryTechName)
        {
            ItemTemplateCategoryTechName = itemTemplateCategoryTechName;
        }
    }
}