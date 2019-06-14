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
        IGroupRepository Groups { get; }
        IItemTemplateRepository ItemTemplates { get; }
        IItemTemplateCategoryRepository ItemTemplateCategories { get; }
        IItemTemplateSectionRepository ItemTemplateSections { get; }
        IJobRepository Jobs { get; }
        ILocationRepository Locations { get; }
        ILootRepository Loots { get; }
        IMonsterTypeRepository MonsterTypes { get; }
        IMonsterCategoryRepository MonsterCategories { get; }
        IMonsterTemplateRepository MonsterTemplates { get; }
        IMonsterTraitRepository MonsterTraits { get; }
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
        }

        public IEffectRepository Effects => new EffectRepository(_naheulbookDbContext);
        public IEffectTypeRepository EffectTypes => new EffectTypeRepository(_naheulbookDbContext);
        public IEffectCategoryRepository EffectCategories => new EffectCategoryRepository(_naheulbookDbContext);
        public IGroupRepository Groups => new GroupRepository(_naheulbookDbContext);
        public IItemTemplateRepository ItemTemplates => new ItemTemplateRepository(_naheulbookDbContext);
        public IItemTemplateSectionRepository ItemTemplateSections => new ItemTemplateSectionRepository(_naheulbookDbContext);
        public IItemTemplateCategoryRepository ItemTemplateCategories => new ItemTemplateCategoryRepository(_naheulbookDbContext);
        public IJobRepository Jobs => new JobRepository(_naheulbookDbContext);
        public ILocationRepository Locations => new LocationRepository(_naheulbookDbContext);
        public ILootRepository Loots=> new LootRepository(_naheulbookDbContext);
        public IMonsterTypeRepository MonsterTypes => new MonsterTypeRepository(_naheulbookDbContext);
        public IMonsterCategoryRepository MonsterCategories => new MonsterCategoryRepository(_naheulbookDbContext);
        public IMonsterTemplateRepository MonsterTemplates => new MonsterTemplateRepository(_naheulbookDbContext);
        public IMonsterTraitRepository MonsterTraits => new MonsterTraitRepository(_naheulbookDbContext);
        public IOriginRepository Origins => new OriginRepository(_naheulbookDbContext);
        public ISkillRepository Skills => new SkillRepository(_naheulbookDbContext);
        public ISlotRepository Slots => new SlotRepository(_naheulbookDbContext);
        public IUserRepository Users => new UserRepository(_naheulbookDbContext);

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