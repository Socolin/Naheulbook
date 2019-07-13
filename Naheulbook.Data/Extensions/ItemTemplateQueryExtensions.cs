using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Extensions
{
    public static class ItemTemplateQueryExtensions
    {
        public static IQueryable<TEntity> IncludeItemTemplateDetails<TEntity>(
            this IQueryable<TEntity> queryable,
            Expression<Func<TEntity, ItemTemplate>> navigationPropertyPath
        ) where TEntity : class
        {
            return queryable
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.UnSkills)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.Skills)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.Modifiers)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.SkillModifiers)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.Requirements)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.Slots)
                .ThenInclude(i => i.Slot)
                .Include(navigationPropertyPath)
                .ThenInclude(i => i.Modifiers);
        }
    }
}