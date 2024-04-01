using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services;

public interface IEffectService
{
    Task<EffectEntity> GetEffectAsync(int effectId);
    Task<ICollection<EffectTypeEntity>> GetEffectSubCategoriesAsync();
    Task<ICollection<EffectEntity>> GetEffectsBySubCategoryAsync(long subCategoryId);
    Task<EffectTypeEntity> CreateEffectTypeAsync(NaheulbookExecutionContext executionContext, CreateEffectTypeRequest request);
    Task<EffectSubCategoryEntity> CreateEffectSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateEffectSubCategoryRequest request);
    Task<EffectEntity> CreateEffectAsync(NaheulbookExecutionContext executionContext, int subCategoryId, CreateEffectRequest request);
    Task<EffectEntity> EditEffectAsync(NaheulbookExecutionContext executionContext, int effectId, EditEffectRequest request);
    Task<List<EffectEntity>> SearchEffectsAsync(string filter);
}

public class EffectService(IUnitOfWorkFactory unitOfWorkFactory, IAuthorizationUtil authorizationUtil)
    : IEffectService
{
    public async Task<EffectEntity> GetEffectAsync(int effectId)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var effect = await uow.Effects.GetWithModifiersAsync(effectId);
            if (effect == null)
                throw new EffectNotFoundException();

            return effect;
        }
    }

    public async Task<ICollection<EffectTypeEntity>> GetEffectSubCategoriesAsync()
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.Effects.GetCategoriesAsync();
        }
    }

    public async Task<ICollection<EffectEntity>> GetEffectsBySubCategoryAsync(long subCategoryId)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.Effects.GetBySubCategoryWithModifiersAsync(subCategoryId);
        }
    }

    public async Task<EffectTypeEntity> CreateEffectTypeAsync(NaheulbookExecutionContext executionContext, CreateEffectTypeRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        var effectType = new EffectTypeEntity
        {
            Name = request.Name,
            SubCategories = new List<EffectSubCategoryEntity>(),
        };

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            uow.EffectTypes.Add(effectType);
            await uow.SaveChangesAsync();
        }

        return effectType;
    }

    public async Task<EffectSubCategoryEntity> CreateEffectSubCategoryAsync(
        NaheulbookExecutionContext executionContext,
        CreateEffectSubCategoryRequest request
    )
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        var effectSubCategory = new EffectSubCategoryEntity
        {
            Name = request.Name,
            TypeId = request.TypeId,
            DiceSize = request.DiceSize,
            DiceCount = request.DiceCount,
            Note = request.Note,
            Effects = new List<EffectEntity>(),
        };

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            uow.EffectSubCategories.Add(effectSubCategory);
            await uow.SaveChangesAsync();
        }

        return effectSubCategory;
    }

    public async Task<EffectEntity> CreateEffectAsync(NaheulbookExecutionContext executionContext, int subCategoryId, CreateEffectRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        var effect = new EffectEntity
        {
            Name = request.Name,
            SubCategoryId = subCategoryId,
            Description = request.Description,
            Dice = request.Dice,
            TimeDuration = request.TimeDuration,
            CombatCount = request.CombatCount,
            Duration = request.Duration,
            LapCount = request.LapCount,
            DurationType = request.DurationType,
            Modifiers = request.Modifiers.Select(s => new EffectModifierEntity
            {
                StatName = s.Stat, Type = s.Type, Value = s.Value,
            }).ToList(),
        };

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            uow.Effects.Add(effect);
            await uow.SaveChangesAsync();
        }

        return effect;
    }

    public async Task<EffectEntity> EditEffectAsync(NaheulbookExecutionContext executionContext, int effectId, EditEffectRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var effect = await uow.Effects.GetWithModifiersAsync(effectId);
            if (effect == null)
                throw new EffectNotFoundException();

            effect.Name = request.Name;
            effect.SubCategoryId = request.SubCategoryId;
            effect.Description = request.Description;
            effect.Dice = request.Dice;
            effect.TimeDuration = request.TimeDuration;
            effect.CombatCount = request.CombatCount;
            effect.Duration = request.Duration;
            effect.LapCount = request.LapCount;
            effect.DurationType = request.DurationType;
            effect.Modifiers = request.Modifiers.Select(s => new EffectModifierEntity
            {
                StatName = s.Stat, Type = s.Type, Value = s.Value,
            }).ToList();

            await uow.SaveChangesAsync();

            return effect;
        }
    }

    public async Task<List<EffectEntity>> SearchEffectsAsync(string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return new List<EffectEntity>();
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.Effects.SearchByNameAsync(filter, 10);
        }
    }
}