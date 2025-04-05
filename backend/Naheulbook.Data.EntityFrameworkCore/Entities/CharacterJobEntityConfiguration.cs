using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterJobEntityConfiguration : IEntityTypeConfiguration<CharacterJobEntity>
{
    public void Configure(EntityTypeBuilder<CharacterJobEntity> builder)
    {
        builder.ToTable("character_jobs");

        builder.HasKey(e => new {e.CharacterId, e.JobId});

        builder.HasIndex(e => e.JobId)
            .HasDatabaseName("IX_character_jobs_jobId");

        builder.HasIndex(e => e.CharacterId)
            .HasDatabaseName("IX_character_jobs_characterId");

        builder.Property(e => e.CharacterId)
            .HasColumnName("characterId");

        builder.Property(e => e.JobId)
            .HasColumnName("jobId");

        builder.Property(e => e.Order)
            .HasColumnName("order");

        builder.HasOne(e => e.Character)
            .WithMany(e => e.Jobs)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_character_jobs_characterId_characters_id");

        builder.HasOne(e => e.Job)
            .WithMany()
            .HasForeignKey(e => e.JobId)
            .HasConstraintName("FK_character_jobs_jobId_jobs_id");
    }
}