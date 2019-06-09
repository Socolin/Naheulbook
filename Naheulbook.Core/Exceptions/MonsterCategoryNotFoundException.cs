using System;

namespace Naheulbook.Core.Exceptions
{
    public class MonsterCategoryNotFoundException : Exception
    {
        public int CategoryId { get; }

        public MonsterCategoryNotFoundException(int categoryId)
        {
            CategoryId = categoryId;
        }
    }
}