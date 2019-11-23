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
        public DbSet<ItemTemplateSubCategory> ItemTemplateSubCategories { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<Map> Maps { get; set; } = null!;
        public DbSet<MapLayer> MapLayers { get; set; } = null!;
        public DbSet<MapMarker> MapMarkers { get; set; } = null!;
        public DbSet<MapMarkerLink> MapMarkerLinks { get; set; } = null!;
        public DbSet<Monster> Monsters { get; set; } = null!;
        public DbSet<MonsterType> MonsterTypes { get; set; } = null!;
        public DbSet<MonsterTemplate> MonsterTemplates { get; set; } = null!;
        public DbSet<Npc> Npcs { get; set; } = null!;
        public DbSet<Origin> Origins { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<Speciality> Specialities { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
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
            modelBuilder.ApplyConfiguration(new EffectSubCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new EffectModifierConfiguration());
            modelBuilder.ApplyConfiguration(new EffectTypeConfiguration());

            modelBuilder.ApplyConfiguration(new EventConfiguration());

            modelBuilder.ApplyConfiguration(new GodConfiguration());

            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new GroupHistoryEntryConfiguration());
            modelBuilder.ApplyConfiguration(new GroupInviteConfiguration());

            modelBuilder.ApplyConfiguration(new ItemConfiguration());

            modelBuilder.ApplyConfiguration(new ItemTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateSubCategoryConfiguration());
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
            modelBuilder.ApplyConfiguration(new JobRestrictionConfiguration());
            modelBuilder.ApplyConfiguration(new JobSkillConfiguration());

            modelBuilder.ApplyConfiguration(new LootConfiguration());

            modelBuilder.ApplyConfiguration(new MapConfiguration());
            modelBuilder.ApplyConfiguration(new MapLayerConfiguration());
            modelBuilder.ApplyConfiguration(new MapMarkerConfiguration());
            modelBuilder.ApplyConfiguration(new MapMarkerLinkConfiguration());

            modelBuilder.ApplyConfiguration(new MonsterConfiguration());

            modelBuilder.ApplyConfiguration(new MonsterSubCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTemplateInventoryElementConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MonsterTraitConfiguration());

            modelBuilder.ApplyConfiguration(new NpcConfiguration());

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