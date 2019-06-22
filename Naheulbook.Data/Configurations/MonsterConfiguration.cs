using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class MonsterConfiguration : IEntityTypeConfiguration<Monster>
    {
        public void Configure(EntityTypeBuilder<Monster> builder)
        {
            builder.ToTable("monster");

            builder.HasKey(x => x.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.HasIndex(x => x.GroupId);
            builder.HasIndex(x => x.LootId);
            builder.HasIndex(x => x.TargetedMonsterId);
            builder.HasIndex(x => x.TargetedCharacterId);

            builder.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("json");

            builder.Property(e => e.Name)
                .HasColumnName("name");

            builder.Property(e => e.Dead)
                .HasColumnName("dead");

            builder.Property(e => e.Modifiers)
                .HasColumnName("modifiers")
                .HasColumnType("json");

            builder.Property(e => e.GroupId)
                .IsRequired()
                .HasColumnName("group");

            builder.Property(e => e.LootId)
                .IsRequired(false)
                .HasColumnName("lootid");

            builder.Property(e => e.TargetedMonsterId)
                .IsRequired(false)
                .HasColumnName("targetmonsterid");

            builder.Property(e => e.TargetedCharacterId)
                .IsRequired(false)
                .HasColumnName("targetcharacterid");

            builder.HasOne(x => x.Group)
                .WithMany(x => x.Monsters)
                .HasForeignKey(x => x.GroupId)
                .HasConstraintName("FK_monster_group_group");

            builder.HasOne(x => x.Loot)
                .WithMany(x => x.Monsters)
                .HasForeignKey(x => x.LootId)
                .HasConstraintName("FK_monster_loot_lootid");

            builder.HasOne(x => x.TargetedMonster)
                .WithMany()
                .HasForeignKey(x => x.TargetedMonsterId)
                .HasConstraintName("FK_monster_monster_targetmonsterid");

            builder.HasOne(x => x.TargetedCharacter)
                .WithMany()
                .HasForeignKey(x => x.TargetedCharacterId)
                .HasConstraintName("FK_monster_character_targetcharacterid");
        }
    }
}