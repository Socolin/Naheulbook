using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.Configurations;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.DbContexts;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class NaheulbookDbContext(DbContextOptions<NaheulbookDbContext> options) : DbContext(options)
{
    public DbSet<CharacterEntity> Characters { get; set; } = null!;
    public DbSet<CharacterModifierEntity> CharacterModifiers { get; set; } = null!;
    public DbSet<CharacterHistoryEntryEntity> CharacterHistory { get; set; } = null!;
    public DbSet<EffectEntity> Effects { get; set; } = null!;
    public DbSet<EffectTypeEntity> EffectTypes { get; set; } = null!;
    public DbSet<GroupEntity> Groups { get; set; } = null!;
    public DbSet<GroupHistoryEntryEntity> GroupHistory { get; set; } = null!;
    public DbSet<GroupInviteEntity> GroupInvites { get; set; } = null!;
    public DbSet<ItemTemplateSectionEntity> ItemTemplateSections { get; set; } = null!;
    public DbSet<ItemEntity> Items { get; set; } = null!;
    public DbSet<ItemTemplateEntity> ItemTemplates { get; set; } = null!;
    public DbSet<ItemTemplateSubCategoryEntity> ItemTemplateSubCategories { get; set; } = null!;
    public DbSet<SkillEntity> Skills { get; set; } = null!;
    public DbSet<MapEntity> Maps { get; set; } = null!;
    public DbSet<MapLayerEntity> MapLayers { get; set; } = null!;
    public DbSet<MapMarkerEntity> MapMarkers { get; set; } = null!;
    public DbSet<MapMarkerLinkEntity> MapMarkerLinks { get; set; } = null!;
    public DbSet<MonsterEntity> Monsters { get; set; } = null!;
    public DbSet<MonsterTypeEntity> MonsterTypes { get; set; } = null!;
    public DbSet<MonsterTemplateEntity> MonsterTemplates { get; set; } = null!;
    public DbSet<NpcEntity> Npcs { get; set; } = null!;
    public DbSet<OriginEntity> Origins { get; set; } = null!;
    public DbSet<JobEntity> Jobs { get; set; } = null!;
    public DbSet<SpecialityEntity> Specialities { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<UserAccessTokenEntity> UserAccessTokens { get; set; } = null!;
    public DbSet<LootEntity> Loots { get; set; } = null!;
    public DbSet<EventEntity> Events { get; set; } = null!;
    public DbSet<OriginRandomNameUrlEntity> OriginRandomNameUrls { get; set; } = null!;
    public DbSet<FightEntity> Fights { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // FIXME: Use assembly
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

        modelBuilder.ApplyConfiguration(new FightConfiguration());

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

        modelBuilder.ApplyConfiguration(new MerchantConfiguration());

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
        modelBuilder.ApplyConfiguration(new UserAccessTokenConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}