#nullable enable
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.TestUtils;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class RepositoryTestsBase<TDbContext> where TDbContext : DbContext
{
    protected TDbContext RepositoryDbContext => _repositoryDbContext ?? throw new NullReferenceException($"{nameof(_repositoryDbContext)} was not initialized");
    private TDbContext? _repositoryDbContext;
    protected TestDataUtil TestDataUtil = null!;

    [SetUp]
    public void BaseSetUp()
    {
        _repositoryDbContext = DbUtils.GetTestDbContext<TDbContext>(true);
        TestDataUtil = new TestDataUtil(DbUtils.GetDbContextOptions<NaheulbookDbContext>());
        TestDataUtil.Cleanup();
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        if (_repositoryDbContext != null)
            await _repositoryDbContext.DisposeAsync();

        await using var dbContext = DbUtils.GetTestDbContext<TDbContext>();
        await dbContext.SaveChangesAsync();
    }

    protected void AssertEntityIsLoaded<T>([NotNull] T? actualEntity, T expectedEntity)
        where T : class
    {
        actualEntity.Should().BeEquivalentTo(expectedEntity, GetEquivalentOptionsForEntity);
        if (actualEntity == null) throw new NullReferenceException("actualEntity is null");
    }

    protected void AssertEntitiesAreLoaded<T>(IEnumerable<T> actualEntities, IEnumerable<T> expectedEntities)
        where T : class
    {
        actualEntities.Should().BeEquivalentTo(expectedEntities, GetEquivalentOptionsForEntity);
    }

    protected void AssertEntitiesAreLoadedWithSameOrder<T>(IEnumerable<T> actualEntities, IEnumerable<T> expectedEntities)
        where T : class
    {
        actualEntities.Should().BeEquivalentTo(expectedEntities, options => GetEquivalentOptionsForEntity(options).WithStrictOrdering());
    }

    private EquivalencyOptions<T> GetEquivalentOptionsForEntity<T>(EquivalencyOptions<T> options)
        where T : class
    {
        EquivalencyOptions<T> equivalencyOptions;
        switch (options)
        {
            case EquivalencyOptions<AptitudeEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.AptitudeGroup)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<AptitudeGroupEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Aptitudes)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<CharacterJobEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Character)
                    .Excluding(x => x.Job)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<CharacterModifierEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Character)
                    .Excluding(x => x.Values)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<CharacterModifierValueEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.CharacterModifierId)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<CharacterSkillEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Character)
                    .Excluding(x => x.Skill)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<CharacterSpecialityEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Character)
                    .Excluding(x => x.Speciality)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<CharacterEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Owner)
                    .Excluding(x => x.Origin)
                    .Excluding(x => x.Group)
                    .Excluding(x => x.TargetedCharacter)
                    .Excluding(x => x.TargetedMonster)
                    .Excluding(x => x.Jobs)
                    .Excluding(x => x.Modifiers)
                    .Excluding(x => x.Skills)
                    .Excluding(x => x.Specialities)
                    .Excluding(x => x.Items)
                    .Excluding(x => x.Invites)
                    .Excluding(x => x.HistoryEntries)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<GroupEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.CombatLoot)
                    .Excluding(x => x.Master)
                    .Excluding(x => x.Loots)
                    .Excluding(x => x.Monsters)
                    .Excluding(x => x.Characters)
                    .Excluding(x => x.Invites)
                    .Excluding(x => x.Events)
                    .Excluding(x => x.Fights)
                    .Excluding(x => x.HistoryEntries)
                    .Excluding(x => x.Npcs)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<GroupInviteEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Character)
                    .Excluding(x => x.Group)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<EffectEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.SubCategory)
                    .Excluding(x => x.Modifiers)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<EffectModifierEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Effect)
                    .Excluding(x => x.Stat)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<EffectTypeEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.SubCategories)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<EffectSubCategoryEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Type)
                    .Excluding(x => x.Effects)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<IHistoryEntry> entityOptions:
                equivalencyOptions = entityOptions
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<CharacterHistoryEntryEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Character)
                    .Excluding(x => x.CharacterModifier)
                    .Excluding(x => x.Effect)
                    .Excluding(x => x.Item)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Container)
                    .Excluding(x => x.ItemTemplate)
                    .Excluding(x => x.Character)
                    .Excluding(x => x.Loot)
                    .Excluding(x => x.Monster)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemTemplateEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.SubCategory)
                    .Excluding(x => x.SourceUser)
                    .Excluding(x => x.Modifiers)
                    .Excluding(x => x.Requirements)
                    .Excluding(x => x.SkillModifiers)
                    .Excluding(x => x.Slots)
                    .Excluding(x => x.Skills)
                    .Excluding(x => x.UnSkills)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemTemplateModifierEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.ItemTemplate)
                    .Excluding(x => x.RequiredJob)
                    .Excluding(x => x.RequiredOrigin)
                    .Excluding(x => x.Stat)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemTemplateRequirementEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.ItemTemplate)
                    .Excluding(x => x.Stat)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemTemplateSkillEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.ItemTemplate)
                    .Excluding(x => x.Skill)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemTemplateSkillModifierEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.ItemTemplate)
                    .Excluding(x => x.Skill)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemTemplateUnSkillEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.ItemTemplate)
                    .Excluding(x => x.Skill)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<ItemTemplateSlotEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.ItemTemplate)
                    .Excluding(x => x.Slot)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<JobEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Bonuses)
                    .Excluding(x => x.Information)
                    .Excluding(x => x.Requirements)
                    .Excluding(x => x.Restrictions)
                    .Excluding(x => x.Skills)
                    .Excluding(x => x.Specialities)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<JobBonusEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Job)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<JobRequirementEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Job)
                    .Excluding(x => x.Stat)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<JobRestrictionEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Job)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<JobSkillEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Job)
                    .Excluding(x => x.Skill)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<LootEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Group)
                    .Excluding(x => x.Monsters)
                    .Excluding(x => x.Items)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<MonsterEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Fight)
                    .Excluding(x => x.Group)
                    .Excluding(x => x.Items)
                    .Excluding(x => x.Loot)
                    .Excluding(x => x.TargetedCharacter)
                    .Excluding(x => x.TargetedMonster)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<OriginEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Bonuses)
                    .Excluding(x => x.AptitudeGroup)
                    .Excluding(x => x.Information)
                    .Excluding(x => x.Requirements)
                    .Excluding(x => x.Restrictions)
                    .Excluding(x => x.Skills)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<SlotEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<SpecialityEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Job)
                    .Excluding(x => x.Modifiers)
                    .Excluding(x => x.Specials)
                    .As<EquivalencyOptions<T>>();
                break;
            case EquivalencyOptions<UserEntity> entityOptions:
                equivalencyOptions = entityOptions
                    .Excluding(x => x.Characters)
                    .Excluding(x => x.Groups)
                    .As<EquivalencyOptions<T>>();
                break;
            default:
                throw new NotSupportedException($"{typeof(T).Name} is not supported. Add missing `case`");
        }

        return equivalencyOptions
            .PreferringRuntimeMemberTypes();
    }
}