using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(39)]
public class Mig0039AddFightIdOnMonster : AutoReversingMigration
{
    public override void Up()
    {
        Alter.Table("monsters")
            .AddColumn("fightId").AsInt64().Nullable();
        Create.ForeignKey("FK_monsters_fightId_fights_id")
            .FromTable("monsters").ForeignColumns("fightId")
            .ToTable("fights").PrimaryColumn("id")
            .OnDelete(Rule.Cascade);
    }
}