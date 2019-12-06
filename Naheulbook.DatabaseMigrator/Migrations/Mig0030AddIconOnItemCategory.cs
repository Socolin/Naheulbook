using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(30)]
    public class Mig0030AddIconOnItemCategory : Migration
    {
        public override void Up()
        {
            Alter.Table("item_template_sections").AddColumn("icon").AsString(64).SetExistingRowsTo("uncertainty");
        }

        public override void Down()
        {
            Delete.Column("icon").FromTable("item_template_sections");
        }
    }
}
