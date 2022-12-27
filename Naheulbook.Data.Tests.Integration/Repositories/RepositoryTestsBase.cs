#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.Models;
using Naheulbook.TestUtils;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class RepositoryTestsBase<TDbContext> where TDbContext : DbContext
    {
        protected TDbContext RepositoryDbContext => _repositoryDbContext ?? throw new NullReferenceException($"{nameof(_repositoryDbContext)} was not initialized");
        private TDbContext? _repositoryDbContext;
        protected TestDataUtil TestDataUtil = null!;

        [SetUp]
        public void BaseSetUp()
        {
            _repositoryDbContext = DbUtils.GetTestDbContext<TDbContext>(true);
            TestDataUtil = new TestDataUtil(DbUtils.GetDbContextOptions(), new DefaultEntityCreator());
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

        private EquivalencyAssertionOptions<T> GetEquivalentOptionsForEntity<T>(EquivalencyAssertionOptions<T> options)
            where T : class
        {
            EquivalencyAssertionOptions<T> equivalencyAssertionOptions;
            switch (options)
            {
                case EquivalencyAssertionOptions<CharacterJobEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Character)
                        .Excluding(x => x.Job)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<CharacterEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
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
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<GroupEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.CombatLoot)
                        .Excluding(x => x.Master)
                        .Excluding(x => x.Loots)
                        .Excluding(x => x.Monsters)
                        .Excluding(x => x.Characters)
                        .Excluding(x => x.Invites)
                        .Excluding(x => x.Events)
                        .Excluding(x => x.HistoryEntries)
                        .Excluding(x => x.Npcs)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<GroupInviteEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Character)
                        .Excluding(x => x.Group)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<EffectEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.SubCategory)
                        .Excluding(x => x.Modifiers)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<EffectModifierEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Effect)
                        .Excluding(x => x.Stat)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<EffectTypeEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.SubCategories)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<EffectSubCategoryEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Type)
                        .Excluding(x => x.Effects)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<IHistoryEntry> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<CharacterHistoryEntryEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Character)
                        .Excluding(x => x.CharacterModifier)
                        .Excluding(x => x.Effect)
                        .Excluding(x => x.Item)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Container)
                        .Excluding(x => x.ItemTemplate)
                        .Excluding(x => x.Character)
                        .Excluding(x => x.Loot)
                        .Excluding(x => x.Monster)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemTemplateEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.SubCategory)
                        .Excluding(x => x.SourceUser)
                        .Excluding(x => x.Modifiers)
                        .Excluding(x => x.Requirements)
                        .Excluding(x => x.SkillModifiers)
                        .Excluding(x => x.Slots)
                        .Excluding(x => x.Skills)
                        .Excluding(x => x.UnSkills)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemTemplateModifierEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.ItemTemplate)
                        .Excluding(x => x.RequiredJob)
                        .Excluding(x => x.RequiredOrigin)
                        .Excluding(x => x.Stat)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemTemplateRequirementEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.ItemTemplate)
                        .Excluding(x => x.Stat)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemTemplateSkillEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.ItemTemplate)
                        .Excluding(x => x.Skill)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemTemplateSkillModifierEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.ItemTemplate)
                        .Excluding(x => x.Skill)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemTemplateUnSkillEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.ItemTemplate)
                        .Excluding(x => x.Skill)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<ItemTemplateSlotEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.ItemTemplate)
                        .Excluding(x => x.Slot)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<JobEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Bonuses)
                        .Excluding(x => x.Information)
                        .Excluding(x => x.Requirements)
                        .Excluding(x => x.Restrictions)
                        .Excluding(x => x.Skills)
                        .Excluding(x => x.Specialities)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<JobBonusEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Job)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<JobRequirementEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Job)
                        .Excluding(x => x.Stat)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<JobRestrictionEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Job)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<JobSkillEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Job)
                        .Excluding(x => x.Skill)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<OriginEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Bonuses)
                        .Excluding(x => x.Information)
                        .Excluding(x => x.Requirements)
                        .Excluding(x => x.Restrictions)
                        .Excluding(x => x.Skills)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<SlotEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<SpecialityEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Job)
                        .Excluding(x => x.Modifiers)
                        .Excluding(x => x.Specials)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                case EquivalencyAssertionOptions<UserEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Characters)
                        .Excluding(x => x.Groups)
                        .As<EquivalencyAssertionOptions<T>>();
                    break;
                default:
                    throw new NotSupportedException($"{typeof(T).Name} is not supported. Add missing `case`");
            }

            return equivalencyAssertionOptions
                .RespectingRuntimeTypes();
        }
    }
}