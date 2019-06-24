using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.Configurations;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.DbContexts
{
    public class NaheulbookDbContext : DbContext
    {
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterHistoryEntry> CharacterHistory { get; set; }
        public DbSet<Effect> Effects { get; set; }
        public DbSet<EffectType> EffectTypes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupHistoryEntry> GroupHistory { get; set; }
        public DbSet<GroupInvite> GroupInvites { get; set; }
        public DbSet<ItemTemplateSection> ItemTemplateSections { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemTemplate> ItemTemplates { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<MonsterType> MonsterTypes { get; set; }
        public DbSet<MonsterTemplate> MonsterTemplates { get; set; }
        public DbSet<Origin> Origins { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Loot> Loots { get; set; }
        public DbSet<Event> Events { get; set; }

        public NaheulbookDbContext(DbContextOptions<NaheulbookDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CalendarConfiguration());

            modelBuilder.ApplyConfiguration(new CharacterConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterJobConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterModifierConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterModifierValueConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterSkillConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterSpecialityConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterHistoryEntryConfiguration());

            modelBuilder.ApplyConfiguration(new EffectConfiguration());
            modelBuilder.ApplyConfiguration(new EffectCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new EffectModifierConfiguration());
            modelBuilder.ApplyConfiguration(new EffectTypeConfiguration());

            modelBuilder.ApplyConfiguration(new EventConfiguration());

            modelBuilder.ApplyConfiguration(new GodConfiguration());

            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new GroupHistoryEntryConfiguration());
            modelBuilder.ApplyConfiguration(new GroupInviteConfiguration());

            modelBuilder.ApplyConfiguration(new ItemConfiguration());

            modelBuilder.ApplyConfiguration(new ItemTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateModifierConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateRequirementConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateSectionConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateSkillConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateSkillModifiersConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateSlotConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateUnSkillConfiguration());
            modelBuilder.ApplyConfiguration(new SlotConfiguration());

            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new JobBonusConfiguration());
            modelBuilder.ApplyConfiguration(new JobOriginBlacklistConfiguration());
            modelBuilder.ApplyConfiguration(new JobOriginWhitelistConfiguration());
            modelBuilder.ApplyConfiguration(new JobRequirementConfiguration());
            modelBuilder.ApplyConfiguration(new JobRestrictConfiguration());
            modelBuilder.ApplyConfiguration(new JobSkillConfiguration());

            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new LocationMapConfiguration());

            modelBuilder.ApplyConfiguration(new LootConfiguration());

            modelBuilder.ApplyConfiguration(new MonsterConfiguration());

            modelBuilder.ApplyConfiguration(new MonsterCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterLocationConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTemplateSimpleInventoryConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTraitConfiguration());

            modelBuilder.ApplyConfiguration(new OriginConfiguration());
            modelBuilder.ApplyConfiguration(new OriginBonusConfiguration());
            modelBuilder.ApplyConfiguration(new OriginInfoConfiguration());
            modelBuilder.ApplyConfiguration(new OriginRequirementConfiguration());
            modelBuilder.ApplyConfiguration(new OriginRestrictConfiguration());
            modelBuilder.ApplyConfiguration(new OriginSkillConfiguration());

            modelBuilder.ApplyConfiguration(new SkillConfiguration());
            modelBuilder.ApplyConfiguration(new SkillEffectConfiguration());

            modelBuilder.ApplyConfiguration(new SpecialityConfiguration());
            modelBuilder.ApplyConfiguration(new SpecialityModifierConfiguration());
            modelBuilder.ApplyConfiguration(new SpecialitySpecialConfiguration());

            modelBuilder.ApplyConfiguration(new StatConfiguration());

            modelBuilder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}