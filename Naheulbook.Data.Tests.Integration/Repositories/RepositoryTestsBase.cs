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
                case EquivalencyAssertionOptions<OriginEntity> entityOptions:
                    equivalencyAssertionOptions = entityOptions
                        .Excluding(x => x.Bonuses)
                        .Excluding(x => x.Information)
                        .Excluding(x => x.Requirements)
                        .Excluding(x => x.Restrictions)
                        .Excluding(x => x.Skills)
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