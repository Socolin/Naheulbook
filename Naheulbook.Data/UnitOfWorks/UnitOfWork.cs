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
        IItemTemplateRepository ItemTemplates { get; }
        IItemTemplateCategoryRepository ItemTemplateCategories { get; }
        IItemTemplateSectionRepository ItemTemplateSections { get; }
        IJobRepository Jobs { get; }
        ILocationRepository Locations { get; }
        IMonsterTypeRepository  MonsterTypes { get; }
        IMonsterCategoryRepository MonsterCategories { get; }
        IMonsterTemplateRepository MonsterTemplates { get; }
        IOriginRepository Origins { get; }
        ISkillRepository Skills { get; }
        ISlotRepository Slots { get; }
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
            ItemTemplates = new ItemTemplateRepository(naheulbookDbContext);
            ItemTemplateCategories = new ItemTemplateCategoryRepository(naheulbookDbContext);
            ItemTemplateSections = new ItemTemplateSectionRepository(naheulbookDbContext);
            EffectCategories = new EffectCategoryRepository(naheulbookDbContext);
            EffectTypes = new EffectTypeRepository(naheulbookDbContext);
            Jobs = new JobRepository(naheulbookDbContext);
            Locations = new LocationRepository(naheulbookDbContext);
            MonsterTypes = new MonsterTypeRepository(naheulbookDbContext);
            MonsterCategories = new MonsterCategoryRepository(naheulbookDbContext);
            MonsterTemplates = new MonsterTemplateRepository(naheulbookDbContext);
            Origins = new OriginRepository(naheulbookDbContext);
            Skills = new SkillRepository(naheulbookDbContext);
            Slots = new SlotRepository(naheulbookDbContext);
            Users = new UserRepository(naheulbookDbContext);
        }

        public IEffectRepository Effects { get; }
        public IEffectTypeRepository EffectTypes { get; }
        public IEffectCategoryRepository EffectCategories { get; }
        public IItemTemplateRepository ItemTemplates { get; }
        public IItemTemplateCategoryRepository ItemTemplateCategories { get; }
        public IItemTemplateSectionRepository ItemTemplateSections { get; }
        public IJobRepository Jobs { get; }
        public ILocationRepository Locations { get; }
        public IMonsterTypeRepository MonsterTypes { get; }
        public IMonsterCategoryRepository MonsterCategories { get; }
        public IMonsterTemplateRepository MonsterTemplates { get; }
        public IOriginRepository Origins { get; }
        public ISkillRepository Skills { get; }
        public ISlotRepository Slots { get; set; }
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