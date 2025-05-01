using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpecialityModifierEntityConfiguration : IEntityTypeConfiguration<SpecialityModifierEntity>
{
    public void Configure(EntityTypeBuilder<SpecialityModifierEntity> builder)
    {
        builder.ToTable("speciality_modifiers");

        builder.HasIndex(e => e.SpecialityId)
            .HasDatabaseName("IX_speciality_modifiers_specialityId");

        builder.HasIndex(e => e.Stat)
            .HasDatabaseName("IX_speciality_modifier_stat");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.SpecialityId)
            .HasColumnName("specialityId");

        builder.Property(e => e.Stat)
            .IsRequired()
            .HasColumnName("stat")
            .HasMaxLength(64);

        builder.Property(e => e.Value)
            .HasColumnName("value");

        builder.HasOne(e => e.Speciality)
            .WithMany(s => s.Modifiers)
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_speciality_modifiers_specialityId_specialities_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}