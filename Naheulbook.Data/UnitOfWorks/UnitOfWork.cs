using System;
using System.Threading.Tasks;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Repositories;

namespace Naheulbook.Data.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IEffectRepository Effects { get; }
        IEffectCategoryRepository EffectCategories { get; }
        IEffectTypeRepository EffectTypes { get; }
        IJobRepository Jobs { get; }
        IOriginRepository Origins { get; }
        ISkillRepository Skills { get; }
        IUserRepository Users { get; }

        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly NaheulbookDbContext _naheulbookDbContext;

        public UnitOfWork(NaheulbookDbContext naheulbookDbContext)
        {
            _naheulbookDbContext = naheulbookDbContext ?? throw new ArgumentNullException(nameof(naheulbookDbContext));
            Effects = new EffectRepository(naheulbookDbContext);
            EffectCategories = new EffectCategoryRepository(naheulbookDbContext);
            EffectTypes = new EffectTypeRepository(naheulbookDbContext);
            Jobs = new JobRepository(naheulbookDbContext);
            Skills = new SkillRepository(naheulbookDbContext);
            Origins = new OriginRepository(naheulbookDbContext);
            Users = new UserRepository(naheulbookDbContext);
        }

        public IEffectRepository Effects { get; }
        public IEffectTypeRepository EffectTypes { get; }
        public IEffectCategoryRepository EffectCategories { get; }
        public IJobRepository Jobs { get; }
        public IOriginRepository Origins { get; }
        public ISkillRepository Skills { get; }
        public IUserRepository Users { get; }

        public Task<int> CompleteAsync()
        {
            return _naheulbookDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _naheulbookDbContext.Dispose();
        }
    }
}