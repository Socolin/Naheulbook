using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(42)]
public class Mig0042AddCountOnCharacterAptitudeTable : AutoReversingMigration
{
    public override void Up()
    {
        Alter.Table("characters_aptitudes")
            .AddColumn("count").AsInt32().NotNullable().WithDefaultValue(1)
            .AddColumn("active").AsBoolean().NotNullable().WithDefaultValue(false)
            ;
    }
}