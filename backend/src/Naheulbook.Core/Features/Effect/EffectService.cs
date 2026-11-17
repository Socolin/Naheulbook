using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Effect;

public interface IEffectService
{
    Task<EffectEntity> GetEffectAsync(int effectId, CancellationToken cancellationToken = default);
    Task<ICollection<EffectTypeEntity>> GetEffectSubCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ICollection<EffectEntity>> GetEffectsBySubCategoryAsync(long subCategoryId, CancellationToken cancellationToken = default);
    Task<EffectTypeEntity> CreateEffectTypeAsync(NaheulbookExecutionContext executionContext, CreateEffectTypeRequest request, CancellationToken cancellationToken = default);
    Task<EffectSubCategoryEntity> CreateEffectSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateEffectSubCategoryRequest request, CancellationToken cancellationToken = default);
    Task<EffectEntity> CreateEffectAsync(NaheulbookExecutionContext executionContext, int subCategoryId, CreateEffectRequest request, CancellationToken cancellationToken = default);
    Task<EffectEntity> EditEffectAsync(NaheulbookExecutionContext executionContext, int effectId, EditEffectRequest request, CancellationToken cancellationToken = default);
    Task<List<EffectEntity>> SearchEffectsAsync(string filter, CancellationToken cancellationToken = default);
}

public class EffectService(IUnitOfWorkFactory unitOfWorkFactory, IAuthorizationUtil authorizationUtil)
    : IEffectService
{
    public async Task<EffectEntity> GetEffectAsync(int effectId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var effect = await uow.Effects.GetWithModifiersAsync(effectId, cancellationToken);
        if (effect == null)
            throw new EffectNotFoundException();

        return effect;
    }

    public async Task<ICollection<EffectTypeEntity>> GetEffectSubCategoriesAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        return await uow.Effects.GetCategoriesAsync(cancellationToken);
    }

    public async Task<ICollection<EffectEntity>> GetEffectsBySubCategoryAsync(long subCategoryId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Effects.GetBySubCategoryWithModifiersAsync(subCategoryId, cancellationToken);
    }

    public async Task<EffectTypeEntity> CreateEffectTypeAsync(NaheulbookExecutionContext executionContext, CreateEffectTypeRequest request, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        var effectType = new EffectTypeEntity
        {
            Name = request.Name,
            SubCategories = new List<EffectSubCategoryEntity>(),
        };

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        uow.EffectTypes.Add(effectType);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return effectType;
    }

    public async Task<EffectSubCategoryEntity> CreateEffectSubCategoryAsync(
        NaheulbookExecutionContext executionContext,
        CreateEffectSubCategoryRequest request,
        CancellationToken cancellationToken = default
    )
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        var effectSubCategory = new EffectSubCategoryEntity
        {
            Name = request.Name,
            TypeId = request.TypeId,
            DiceSize = request.DiceSize,
            DiceCount = request.DiceCount,
            Note = request.Note,
            Effects = new List<EffectEntity>(),
        };

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        uow.EffectSubCategories.Add(effectSubCategory);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return effectSubCategory;
    }

    public async Task<EffectEntity> CreateEffectAsync(NaheulbookExecutionContext executionContext, int subCategoryId, CreateEffectRequest request, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

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
                }
            ).ToList(),
        };

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        uow.Effects.Add(effect);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return effect;
    }

    public async Task<EffectEntity> EditEffectAsync(NaheulbookExecutionContext executionContext, int effectId, EditEffectRequest request, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var effect = await uow.Effects.GetWithModifiersAsync(effectId, cancellationToken);
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
            }
        ).ToList();

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return effect;
    }

    public async Task<List<EffectEntity>> SearchEffectsAsync(string filter, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filter))
            return [];
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Effects.SearchByNameAsync(filter, 10, cancellationToken);
    }
}