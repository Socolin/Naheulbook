using System;
using System.Threading.Tasks;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Repositories;

namespace Naheulbook.Data.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        ICalendarRepository Calendar { get; }
        ICharacterRepository Characters { get; }
        ICharacterModifierRepository CharacterModifiers { get; }
        ICharacterHistoryEntryRepository CharacterHistoryEntries { get; }
        IEffectRepository Effects { get; }
        IEffectCategoryRepository EffectCategories { get; }
        IEffectTypeRepository EffectTypes { get; }
        IEventRepository Events { get; }
        IGodRepository Gods { get; }
        IGroupRepository Groups { get; }
        IGroupHistoryEntryRepository GroupHistoryEntries { get; }
        IGroupInviteRepository GroupInvites { get; }
        IIconRepository Icons { get; }
        IItemRepository Items { get; }
        IItemTemplateRepository ItemTemplates { get; }
        IItemTemplateCategoryRepository ItemTemplateCategories { get; }
        IItemTemplateSectionRepository ItemTemplateSections { get; }
        IItemTypeRepository ItemTypes { get; }
        IJobRepository Jobs { get; }
        ILocationRepository Locations { get; }
        ILootRepository Loots { get; }
        IMonsterRepository Monsters { get; }
        IMonsterTypeRepository MonsterTypes { get; }
        IMonsterCategoryRepository MonsterCategories { get; }
        IMonsterTemplateRepository MonsterTemplates { get; }
        IMonsterTraitRepository MonsterTraits { get; }
        IOriginRepository Origins { get; }
        ISkillRepository Skills { get; }
        ISlotRepository Slots { get; }
        IStatRepository Stats { get; }
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

        public ICalendarRepository Calendar => new CalendarRepository(_naheulbookDbContext);
        public ICharacterRepository Characters => new CharacterRepository(_naheulbookDbContext);
        public ICharacterModifierRepository CharacterModifiers => new CharacterModifierRepository(_naheulbookDbContext);
        public ICharacterHistoryEntryRepository CharacterHistoryEntries => new CharacterHistoryEntryRepository(_naheulbookDbContext);
        public IEffectRepository Effects => new EffectRepository(_naheulbookDbContext);
        public IEffectTypeRepository EffectTypes => new EffectTypeRepository(_naheulbookDbContext);
        public IEffectCategoryRepository EffectCategories => new EffectCategoryRepository(_naheulbookDbContext);
        public IEventRepository Events => new EventRepository(_naheulbookDbContext);
        public IGodRepository Gods => new GodRepository(_naheulbookDbContext);
        public IGroupRepository Groups => new GroupRepository(_naheulbookDbContext);
        public IGroupHistoryEntryRepository GroupHistoryEntries => new GroupHistoryEntryRepository(_naheulbookDbContext);
        public IGroupInviteRepository GroupInvites => new GroupInviteRepository(_naheulbookDbContext);
        public IIconRepository Icons => new IconRepository(_naheulbookDbContext);
        public IItemRepository Items => new ItemRepository(_naheulbookDbContext);
        public IItemTemplateRepository ItemTemplates => new ItemTemplateRepository(_naheulbookDbContext);
        public IItemTemplateSectionRepository ItemTemplateSections => new ItemTemplateSectionRepository(_naheulbookDbContext);
        public IItemTemplateCategoryRepository ItemTemplateCategories => new ItemTemplateCategoryRepository(_naheulbookDbContext);
        public IItemTypeRepository ItemTypes => new ItemTypeRepository(_naheulbookDbContext);
        public IJobRepository Jobs => new JobRepository(_naheulbookDbContext);
        public ILocationRepository Locations => new LocationRepository(_naheulbookDbContext);
        public ILootRepository Loots => new LootRepository(_naheulbookDbContext);
        public IMonsterRepository Monsters => new MonsterRepository(_naheulbookDbContext);
        public IMonsterTypeRepository MonsterTypes => new MonsterTypeRepository(_naheulbookDbContext);
        public IMonsterCategoryRepository MonsterCategories => new MonsterCategoryRepository(_naheulbookDbContext);
        public IMonsterTemplateRepository MonsterTemplates => new MonsterTemplateRepository(_naheulbookDbContext);
        public IMonsterTraitRepository MonsterTraits => new MonsterTraitRepository(_naheulbookDbContext);
        public IOriginRepository Origins => new OriginRepository(_naheulbookDbContext);
        public ISkillRepository Skills => new SkillRepository(_naheulbookDbContext);
        public ISlotRepository Slots => new SlotRepository(_naheulbookDbContext);
        public IStatRepository Stats => new StatRepository(_naheulbookDbContext);
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