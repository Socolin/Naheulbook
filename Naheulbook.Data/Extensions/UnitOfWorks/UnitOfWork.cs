using System;
using System.Threading.Tasks;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Repositories;

namespace Naheulbook.Data.Extensions.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        ICalendarRepository Calendar { get; }
        ICharacterRepository Characters { get; }
        ICharacterSkillRepository CharacterSkills { get; }
        ICharacterSpecialityRepository CharacterSpecialities { get; }
        ICharacterModifierRepository CharacterModifiers { get; }
        ICharacterHistoryEntryRepository CharacterHistoryEntries { get; }
        IEffectRepository Effects { get; }
        IEffectSubCategoryRepository EffectSubCategories { get; }
        IEffectTypeRepository EffectTypes { get; }
        IEventRepository Events { get; }
        IGodRepository Gods { get; }
        IGroupRepository Groups { get; }
        IGroupHistoryEntryRepository GroupHistoryEntries { get; }
        IGroupInviteRepository GroupInvites { get; }
        IItemRepository Items { get; }
        IItemTemplateRepository ItemTemplates { get; }
        IItemTemplateSubCategoryRepository ItemTemplateSubCategories { get; }
        IItemTemplateSectionRepository ItemTemplateSections { get; }
        IItemTypeRepository ItemTypes { get; }
        IJobRepository Jobs { get; }
        ILootRepository Loots { get; }
        IMapRepository Maps { get; }
        IMapLayerRepository MapLayers { get; }
        IMapMarkerRepository MapMarkers { get; }
        IMapMarkerLinkRepository MapMarkerLinks { get; }
        IMonsterRepository Monsters { get; }
        IMonsterTypeRepository MonsterTypes { get; }
        IMonsterSubCategoryRepository MonsterSubCategories { get; }
        IMonsterTemplateRepository MonsterTemplates { get; }
        IMonsterTraitRepository MonsterTraits { get; }
        INpcRepository Npcs { get; }
        IOriginRepository Origins { get; }
        IOriginRandomNameUrlRepository OriginRandomNameUrls{ get; }
        ISkillRepository Skills { get; }
        ISlotRepository Slots { get; }
        IStatRepository Stats { get; }
        IUserRepository Users { get; }
        IUserAccessTokenRepository UserAccessTokenRepository { get; }

        Task<int> SaveChangesAsync();
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
        public ICharacterSkillRepository CharacterSkills => new CharacterSkillRepository(_naheulbookDbContext);
        public ICharacterSpecialityRepository CharacterSpecialities => new CharacterSpecialityRepository(_naheulbookDbContext);
        public ICharacterModifierRepository CharacterModifiers => new CharacterModifierRepository(_naheulbookDbContext);
        public ICharacterHistoryEntryRepository CharacterHistoryEntries => new CharacterHistoryEntryRepository(_naheulbookDbContext);
        public IEffectRepository Effects => new EffectRepository(_naheulbookDbContext);
        public IEffectTypeRepository EffectTypes => new EffectTypeRepository(_naheulbookDbContext);
        public IEffectSubCategoryRepository EffectSubCategories => new EffectSubCategoryRepository(_naheulbookDbContext);
        public IEventRepository Events => new EventRepository(_naheulbookDbContext);
        public IGodRepository Gods => new GodRepository(_naheulbookDbContext);
        public IGroupRepository Groups => new GroupRepository(_naheulbookDbContext);
        public IGroupHistoryEntryRepository GroupHistoryEntries => new GroupHistoryEntryRepository(_naheulbookDbContext);
        public IGroupInviteRepository GroupInvites => new GroupInviteRepository(_naheulbookDbContext);
        public IItemRepository Items => new ItemRepository(_naheulbookDbContext);
        public IItemTemplateRepository ItemTemplates => new ItemTemplateRepository(_naheulbookDbContext);
        public IItemTemplateSectionRepository ItemTemplateSections => new ItemTemplateSectionRepository(_naheulbookDbContext);
        public IItemTemplateSubCategoryRepository ItemTemplateSubCategories => new ItemTemplateSubCategoryRepository(_naheulbookDbContext);
        public IItemTypeRepository ItemTypes => new ItemTypeRepository(_naheulbookDbContext);
        public IJobRepository Jobs => new JobRepository(_naheulbookDbContext);
        public ILootRepository Loots => new LootRepository(_naheulbookDbContext);
        public IMapRepository Maps => new MapRepository(_naheulbookDbContext);
        public IMapLayerRepository MapLayers => new MapLayerRepository(_naheulbookDbContext);
        public IMapMarkerRepository MapMarkers => new MapMarkerRepository(_naheulbookDbContext);
        public IMapMarkerLinkRepository MapMarkerLinks => new MapMarkerLinkRepository(_naheulbookDbContext);
        public IMonsterRepository Monsters => new MonsterRepository(_naheulbookDbContext);
        public IMonsterTypeRepository MonsterTypes => new MonsterTypeRepository(_naheulbookDbContext);
        public IMonsterSubCategoryRepository MonsterSubCategories => new MonsterSubCategoryRepository(_naheulbookDbContext);
        public IMonsterTemplateRepository MonsterTemplates => new MonsterTemplateRepository(_naheulbookDbContext);
        public IMonsterTraitRepository MonsterTraits => new MonsterTraitRepository(_naheulbookDbContext);
        public INpcRepository Npcs => new NpcRepository(_naheulbookDbContext);
        public IOriginRepository Origins => new OriginRepository(_naheulbookDbContext);
        public IOriginRandomNameUrlRepository OriginRandomNameUrls => new OriginRandomNameUrlRepository(_naheulbookDbContext);
        public ISkillRepository Skills => new SkillRepository(_naheulbookDbContext);
        public ISlotRepository Slots => new SlotRepository(_naheulbookDbContext);
        public IStatRepository Stats => new StatRepository(_naheulbookDbContext);
        public IUserRepository Users => new UserRepository(_naheulbookDbContext);
        public IUserAccessTokenRepository UserAccessTokenRepository => new UserAccessTokenRepository(_naheulbookDbContext);

        public Task<int> SaveChangesAsync()
        {
            return _naheulbookDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _naheulbookDbContext.Dispose();
        }
    }
}