using System;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(26)]
    public class Mig0026FixCharacterJobsPrimaryKey : Migration
    {
        public override void Up()
        {
            Rename.Column("character").OnTable("character_jobs").To("characterId");
            Delete.ForeignKey("character_job_character_id_fk").OnTable("character_jobs");
            Delete.Index("character_job_character_job_uindex").OnTable("character_jobs");

            Create.PrimaryKey("PRIMARY2").OnTable("character_jobs")
                .Columns("jobId", "characterId");

            Create.Index("IX_character_jobs_characterId")
                .OnTable("character_jobs").OnColumn("characterId");

            Create.ForeignKey("FK_character_jobs_characterId_characters_id")
                .FromTable("character_jobs").ForeignColumn("characterId")
                .ToTable("characters").PrimaryColumn("id");
        }

        public override void Down()
        {
            throw new NotSupportedException();
        }
    }
}