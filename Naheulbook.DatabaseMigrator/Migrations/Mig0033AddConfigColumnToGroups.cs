using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(33)]
    public class Mig0033AddConfigColumnToGroups : Migration
    {
        public override void Up()
        {
            Alter.Table("groups").AddColumn("config").AsCustom("json").Nullable();
        }

        public override void Down()
        {
            Delete.Column("config").FromTable("groups");
        }
    }
}