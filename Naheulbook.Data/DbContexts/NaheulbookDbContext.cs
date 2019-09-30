using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.Configurations;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.DbContexts
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class NaheulbookDbContext : DbContext
    {
        public DbSet<Character> Characters { get; set; } = null!;
        public DbSet<CharacterModifier> CharacterModifiers { get; set; } = null!;
        public DbSet<CharacterHistoryEntry> CharacterHistory { get; set; } = null!;
        public DbSet<Effect> Effects { get; set; } = null!;
        public DbSet<EffectType> EffectTypes { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<GroupHistoryEntry> GroupHistory { get; set; } = null!;
        public DbSet<GroupInvite> GroupInvites { get; set; } = null!;
        public DbSet<ItemTemplateSection> ItemTemplateSections { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<ItemTemplate> ItemTemplates { get; set; } = null!;
        public DbSet<ItemTemplateCategory> ItemTemplateCategories { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<Monster> Monsters { get; set; } = null!;
        public DbSet<MonsterType> MonsterTypes { get; set; } = null!;
        public DbSet<MonsterTemplate> MonsterTemplates { get; set; } = null!;
        public DbSet<Origin> Origins { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<Speciality> Specialities { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<LocationMap> LocationMaps { get; set; } = null!;
        public DbSet<Loot> Loots { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<OriginRandomNameUrl> OriginRandomNameUrls { get; set; } = null!;

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

            modelBuilder.ApplyConfiguration(new IconConfiguration());

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
            modelBuilder.ApplyConfiguration(new ItemTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SlotConfiguration());

            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new JobBonusConfiguration());
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
            modelBuilder.ApplyConfiguration(new OriginRandomNameUrlConfiguration());

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