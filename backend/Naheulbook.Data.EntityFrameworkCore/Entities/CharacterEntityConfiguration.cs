using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterEntityConfiguration : IEntityTypeConfiguration<CharacterEntity>
{
    public void Configure(EntityTypeBuilder<CharacterEntity> builder)
    {
        builder.ToTable("characters");

        builder.HasKey(x => x.Id);

        builder.HasIndex(e => e.GroupId)
            .HasDatabaseName("IX_characters_groupId");
        builder.HasIndex(e => e.OriginId)
            .HasDatabaseName("IX_characters_originId");
        builder.HasIndex(e => e.OwnerId)
            .HasDatabaseName("IX_characters_userId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);
        builder.Property(e => e.Sex)
            .IsRequired()
            .HasColumnName("sexe");
        builder.Property(e => e.IsActive)
            .HasColumnName("active");
        builder.Property(e => e.IsNpc)
            .HasColumnName("isnpc");
        builder.Property(e => e.Color)
            .HasDefaultValue("22DD22")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .HasColumnName("color");

        builder.Property(e => e.Ad)
            .HasColumnName("ad");
        builder.Property(e => e.Cha)
            .HasColumnName("cha");
        builder.Property(e => e.Cou)
            .HasColumnName("cou");
        builder.Property(e => e.Fo)
            .HasColumnName("fo");
        builder.Property(e => e.Int)
            .HasColumnName("int");

        builder.Property(e => e.Ev)
            .IsRequired(false)
            .HasColumnName("ev");
        builder.Property(e => e.Ea)
            .IsRequired(false)
            .HasColumnName("ea");

        builder.Property(e => e.Notes)
            .IsRequired(false)
            .HasColumnName("notes");

        builder.Property(e => e.GmData)
            .IsRequired(false)
            .HasColumnName("gmdata");
        builder.Property(e => e.FatePoint)
            .HasColumnName("fatepoint");
        builder.Property(e => e.StatBonusAd)
            .HasColumnName("statbonusad");

        builder.Property(e => e.Level)
            .HasColumnName("level");
        builder.Property(e => e.Experience)
            .HasColumnName("experience");

        builder.Property(e => e.OwnerId)
            .HasColumnName("userId");
        builder.Property(e => e.OriginId)
            .IsRequired()
            .HasColumnName("originId");
        builder.Property(e => e.GroupId)
            .IsRequired(false)
            .HasColumnName("groupId");
        builder.Property(e => e.TargetedCharacterId)
            .IsRequired(false)
            .HasColumnName("targetcharacterid");
        builder.Property(e => e.TargetedMonsterId)
            .IsRequired(false)
            .HasColumnName("targetmonsterid");

        builder.HasOne(e => e.Owner)
            .WithMany(e => e.Characters)
            .HasForeignKey(e => e.OwnerId)
            .HasConstraintName("FK_characters_userId_users_id");

        builder.HasOne(e => e.Origin)
            .WithMany()
            .HasForeignKey(e => e.OriginId)
            .HasConstraintName("FK_characters_originId_origins_id");

        builder.HasOne(e => e.Group!)
            .WithMany(e => e.Characters)
            .HasForeignKey(e => e.GroupId)
            .HasConstraintName("FK_characters_groupId_groups_id");

        builder.HasOne(e => e.TargetedCharacter)
            .WithMany()
            .HasForeignKey(e => e.TargetedCharacterId)
            .HasConstraintName("IX_character_targetcharacterid");

        builder.HasOne(e => e.TargetedMonster)
            .WithMany()
            .HasForeignKey(e => e.TargetedMonsterId)
            .HasConstraintName("FK_character_monster_targetmonsterid");
    }
}