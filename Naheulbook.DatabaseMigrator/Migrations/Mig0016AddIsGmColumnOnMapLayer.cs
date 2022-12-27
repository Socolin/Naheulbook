using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(16)]
public class Mig0016AddIsGmColumnOnMapLayer : Migration
{
    public override void Up()
    {
        Alter.Table("map_layers").AddColumn("isGm").AsBoolean();
    }

    public override void Down()
    {
        Delete.Column("isGm").FromTable("map_layers");
    }
}