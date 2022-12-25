using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Extensions
{
    public static class ItemQueryExtensions
    {
        public static IQueryable<TEntity> IncludeItemDetails<TEntity>(
            this IQueryable<TEntity> queryable,
            Expression<Func<TEntity, IEnumerable<ItemEntity>>> navigationPropertyPath
        ) where TEntity : class
        {
            return queryable.Include(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.UnSkills)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Skills)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Modifiers)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.SkillModifiers)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Requirements)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Slots)
                .ThenInclude(i => i.Slot)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Modifiers);
        }

        public static IQueryable<TEntity> IncludeChildWithItemDetails<TEntity, TChild>(
            this IQueryable<TEntity> queryable,
            Expression<Func<TEntity, IEnumerable<TChild>>> parentNavigationPropertyPath,
            Expression<Func<TChild, IEnumerable<ItemEntity>>> navigationPropertyPath
        )
            where TEntity : class
            where TChild : class
        {
            return queryable
                .Include(parentNavigationPropertyPath)
                .ThenInclude(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.UnSkills)
                .Include(parentNavigationPropertyPath)
                .ThenInclude(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Skills)
                .Include(parentNavigationPropertyPath)
                .ThenInclude(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Modifiers)
                .Include(parentNavigationPropertyPath)
                .ThenInclude(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.SkillModifiers)
                .Include(parentNavigationPropertyPath)
                .ThenInclude(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Requirements)
                .Include(parentNavigationPropertyPath)
                .ThenInclude(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Slots)
                .ThenInclude(i => i.Slot)
                .Include(parentNavigationPropertyPath)
                .ThenInclude(navigationPropertyPath)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Modifiers);
        }
    }
}