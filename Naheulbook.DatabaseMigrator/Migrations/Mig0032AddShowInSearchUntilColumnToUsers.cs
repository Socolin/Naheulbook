using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(32)]
public class Mig0032AddShowInSearchUntilColumnToUsers : Migration
{
    public override void Up()
    {
        Alter.Table("users").AddColumn("showInSearchUntil").AsCustom("timestamp").Nullable();
    }

    public override void Down()
    {
        Delete.Column("showInSearchUntil").FromTable("users");
    }
}