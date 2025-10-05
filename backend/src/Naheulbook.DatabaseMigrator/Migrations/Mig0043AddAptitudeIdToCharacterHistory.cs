using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(43)]
public class Mig0043AddAptitudeIdToCharacterHistory : AutoReversingMigration
{
    public override void Up()
    {
        Alter.Table("character_history_entries")
            .AddColumn("aptitudeId").AsGuid().Nullable().ForeignKey("aptitudes", "id")
            ;
    }
}