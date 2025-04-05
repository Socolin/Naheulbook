using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpecialitySpecialEntityConfiguration : IEntityTypeConfiguration<SpecialitySpecialEntity>
{
    public void Configure(EntityTypeBuilder<SpecialitySpecialEntity> builder)
    {
        builder.ToTable("speciality_specials");

        builder.HasIndex(e => e.SpecialityId)
            .HasDatabaseName("IX_speciality_specials_specialityId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description");

        builder.Property(e => e.IsBonus)
            .HasColumnName("isbonus");

        builder.Property(e => e.SpecialityId)
            .HasColumnName("specialityId");

        builder.Property(e => e.Flags)
            .HasColumnName("flags")
            .HasColumnType("json");

        builder.HasOne(e => e.Speciality)
            .WithMany(s => s.Specials)
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_speciality_specials_specialityId_specialities_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}