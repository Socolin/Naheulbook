// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Naheulbook.Core.Exceptions
{
    public class EmptyItemTemplateCategoryException : Exception
    {
        public int ItemTemplateCategoryId { get; }

        public EmptyItemTemplateCategoryException(int itemTemplateCategoryId)
        {
            ItemTemplateCategoryId = itemTemplateCategoryId;
        }
    }
}