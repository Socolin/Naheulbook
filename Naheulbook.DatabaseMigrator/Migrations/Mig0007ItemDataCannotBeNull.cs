using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(7)]
public class Mig0007ItemDataCannotBeNull : Migration
{
    public override void Up()
    {
        Alter.Column("data").OnTable("item").AsCustom("json").NotNullable();
    }

    public override void Down()
    {
        Alter.Column("data").OnTable("item").AsCustom("json").Nullable();
    }
}