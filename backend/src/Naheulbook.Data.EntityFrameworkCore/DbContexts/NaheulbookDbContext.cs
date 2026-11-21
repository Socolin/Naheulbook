using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.EntityFrameworkCore.DbContexts;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class NaheulbookDbContext(DbContextOptions<NaheulbookDbContext> options) : DbContext(options)
{
    public DbSet<AptitudeEntity> Aptitudes { get; set; } = null!;
    public DbSet<AptitudeGroupEntity> AptitudeGroups { get; set; } = null!;
    public DbSet<CharacterEntity> Characters { get; set; } = null!;
    public DbSet<CharacterAptitudeEntity> CharacterAptitudes { get; set; } = null!;
    public DbSet<CharacterSpecialAttackEntity> CharacterSpecialAttacks { get; set; } = null!;
    public DbSet<CharacterTalentEntity> CharacterTalents { get; set; } = null!;
    public DbSet<CharacterSpellBookEntity> CharacterSpellBooks { get; set; } = null!;
    public DbSet<CharacterMasteryEntity> CharacterMasteries { get; set; } = null!;
    public DbSet<CharacterHandicapEntity> CharacterHandicaps { get; set; } = null!;
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
    public DbSet<SpecialAttackEntity> SpecialAttacks { get; set; } = null!;
    public DbSet<SpecialAttackGroupEntity> SpecialAttackGroups { get; set; } = null!;
    public DbSet<SpellBookEntity> SpellBooks { get; set; } = null!;
    public DbSet<SpellEntity> Spells { get; set; } = null!;
    public DbSet<TalentEntity> Talents { get; set; } = null!;
    public DbSet<TalentGroupEntity> TalentGroups { get; set; } = null!;
    public DbSet<MasteryEntity> Masteries { get; set; } = null!;
    public DbSet<MasteryGroupEntity> MasteryGroups { get; set; } = null!;
    public DbSet<HandicapEntity> Handicaps { get; set; } = null!;
    public DbSet<MankdebolCalamityEntity> MankdebolCalamities { get; set; } = null!;
    public DbSet<DebilibeukBlessingEntity> DebilibeukBlessings { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<UserAccessTokenEntity> UserAccessTokens { get; set; } = null!;
    public DbSet<LootEntity> Loots { get; set; } = null!;
    public DbSet<EventEntity> Events { get; set; } = null!;
    public DbSet<OriginRandomNameUrlEntity> OriginRandomNameUrls { get; set; } = null!;
    public DbSet<FightEntity> Fights { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NaheulbookDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}