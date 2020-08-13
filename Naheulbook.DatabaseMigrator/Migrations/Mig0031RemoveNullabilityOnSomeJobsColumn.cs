using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(31)]
    public class Mig0031RemoveNullabilityOnSomeJobsColumn : Migration
    {
        public override void Up()
        {
            Alter.Column("playerDescription").OnTable("jobs").AsCustom("longtext").NotNullable();
            Alter.Column("playerSummary").OnTable("jobs").AsCustom("text").NotNullable();
        }

        public override void Down()
        {
            Alter.Column("playerDescription").OnTable("jobs").AsCustom("longtext").Nullable();
            Alter.Column("playerSummary").OnTable("jobs").AsCustom("text").Nullable();
        }
    }
}