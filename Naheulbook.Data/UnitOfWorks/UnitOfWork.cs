using System;
using System.Threading.Tasks;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Repositories;

namespace Naheulbook.Data.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IEffectRepository Effects { get; }
        IJobRepository Jobs { get; }
        IOriginRepository Origins { get; }
        ISkillRepository Skills { get; }

        Task<int> CompleteAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly NaheulbookDbContext _naheulbookDbContext;

        public UnitOfWork(NaheulbookDbContext naheulbookDbContext)
        {
            _naheulbookDbContext = naheulbookDbContext ?? throw new ArgumentNullException(nameof(naheulbookDbContext));
            Effects = new EffectRepository(naheulbookDbContext);
            Jobs = new JobRepository(naheulbookDbContext);
            Skills = new SkillRepository(naheulbookDbContext);
            Origins = new OriginRepository(naheulbookDbContext);
        }

        public IEffectRepository Effects { get; }
        public IJobRepository Jobs { get; }
        public IOriginRepository Origins { get; }
        public ISkillRepository Skills { get; }

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