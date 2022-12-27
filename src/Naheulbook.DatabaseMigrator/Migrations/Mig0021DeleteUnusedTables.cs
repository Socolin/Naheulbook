using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(21)]
public class Mig0021DeleteUnusedTables : Migration
{
    public override void Up()
    {
        Delete.Table("error_report");
        Delete.Table("icon");
    }

    public override void Down()
    {
        Create.Table("icon").WithColumn("name").AsString().PrimaryKey();
        Create.Table("error_report").WithColumn("id").AsInt32().Identity().PrimaryKey();
    }
}