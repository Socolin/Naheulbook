using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(11)]
    public class Mig0011AddMapTable : Migration
    {
        public override void Up()
        {
            Create.Table("map")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("name").AsString(255)
                .WithColumn("data").AsCustom("json")
                ;
        }

        public override void Down()
        {
            Delete.Table("map");
        }
    }
}