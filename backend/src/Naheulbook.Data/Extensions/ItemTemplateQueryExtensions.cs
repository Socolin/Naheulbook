using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Extensions;

public static class ItemTemplateQueryExtensions
{
    public static IQueryable<TEntity> IncludeItemTemplateDetails<TEntity>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, ItemTemplateEntity>> navigationPropertyPath
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
            .ThenInclude(i => i.Slot);
    }

    public static IQueryable<ItemTemplateEntity> IncludeItemTemplateDetails(
        this IQueryable<ItemTemplateEntity> queryable
    )
    {
        return queryable
            .Include(i => i.Requirements)
            .Include(i => i.Modifiers)
            .Include(i => i.Slots)
            .ThenInclude(i => i.Slot)
            .Include(i => i.Skills)
            .Include(i => i.UnSkills)
            .Include(i => i.SkillModifiers);
    }

    public static IQueryable<ItemTemplateEntity> FilterCommunityAndPrivateItemTemplates(
        this IQueryable<ItemTemplateEntity> queryable,
        int? currentUserId,
        bool includeCommunityItems
    )
    {
        return queryable.Where(x => x.Source == ItemTemplateEntity.OfficialSourceValue
                                    || (x.Source == ItemTemplateEntity.CommunitySourceValue && includeCommunityItems)
                                    || (x.Source == ItemTemplateEntity.PrivateSourceValue && x.SourceUserId == currentUserId)
        );
    }


    public static IQueryable<TEntity> IncludeChildWithItemTemplateDetails<TEntity, TChild>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, IEnumerable<TChild>>> parentNavigationPropertyPath,
        Expression<Func<TChild, ItemTemplateEntity>> navigationPropertyPath
    )
        where TEntity : class
        where TChild : class
    {
        return queryable
            .Include(parentNavigationPropertyPath)
            .ThenInclude(navigationPropertyPath)
            .ThenInclude(i => i.UnSkills)
            .Include(parentNavigationPropertyPath)
            .ThenInclude(navigationPropertyPath)
            .ThenInclude(i => i.Skills)
            .Include(parentNavigationPropertyPath)
            .ThenInclude(navigationPropertyPath)
            .ThenInclude(i => i.Modifiers)
            .Include(parentNavigationPropertyPath)
            .ThenInclude(navigationPropertyPath)
            .ThenInclude(i => i.SkillModifiers)
            .Include(parentNavigationPropertyPath)
            .ThenInclude(navigationPropertyPath)
            .ThenInclude(i => i.Requirements)
            .Include(parentNavigationPropertyPath)
            .ThenInclude(navigationPropertyPath)
            .ThenInclude(i => i.Slots)
            .ThenInclude(i => i.Slot)
            .Include(parentNavigationPropertyPath)
            .ThenInclude(navigationPropertyPath)
            .ThenInclude(i => i.Modifiers);
    }
}