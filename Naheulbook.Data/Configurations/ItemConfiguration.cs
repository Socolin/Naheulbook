using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("item");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Data)
                .HasColumnName("data");
            builder.Property(e => e.Modifiers)
                .HasColumnName("modifiers");
            builder.Property(e => e.ContainerId)
                .IsRequired(false)
                .HasColumnName("container");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemtemplateid");

            builder.Property(e => e.CharacterId)
                .IsRequired(false)
                .HasColumnName("characterid");
            builder.Property(e => e.MonsterId)
                .IsRequired(false)
                .HasColumnName("monsterid");
            builder.Property(e => e.LootId)
                .IsRequired(false)
                .HasColumnName("lootid");

            builder.Property(e => e.LifetimeType)
                .HasColumnName("lifetimetype")
                .HasMaxLength(30)
                .HasComputedColumnSql("json_unquote(json_extract(`data`,'$.lifetime.durationType'))");

            builder.HasOne(e => e.ItemTemplate)
                .WithMany()
                .HasForeignKey(e => e.ItemTemplateId);

            builder.HasOne(e => e.Container)
                .WithMany()
                .HasForeignKey(e => e.ContainerId);

            builder.HasOne(e => e.Character)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.CharacterId);

            builder.HasOne(e => e.Loot)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.LootId);

            builder.HasOne(e => e.Monster)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.MonsterId);
        }
    }
}