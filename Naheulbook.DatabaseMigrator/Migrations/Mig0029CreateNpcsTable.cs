using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(29)]
    public class Mig0029CreateNpcsTable : Migration
    {
        public override void Up()
        {
            Create.Table("npcs")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("groupId").AsInt64().ForeignKey("FK_npcs_groupId_groups_id", "groups", "id")
                .WithColumn("name").AsString(255)
                .WithColumn("data").AsCustom("json")
                ;
        }

        public override void Down()
        {
            Delete.Table("npcs");
        }
    }
}