using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterAptitudeEntityConfiguration : IEntityTypeConfiguration<CharacterAptitudeEntity>
{
    public void Configure(EntityTypeBuilder<CharacterAptitudeEntity> builder)
    {
        builder.ToTable("characters_aptitudes");

        builder.HasKey(e => new {e.CharacterId, e.AptitudeId});
    }
}