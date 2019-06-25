using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(4)]
    public class Mig0004RemoveUserFromGroupHistoryAndCharacterHistory : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_group_history_user_user").OnTable("group_history");
            Delete.ForeignKey("FK_character_history_user_user").OnTable("character_history");
            Delete.Column("user").FromTable("character_history");
            Delete.Column("user").FromTable("group_history");
        }

        public override void Down()
        {
            Create.Column("user").OnTable("character_history").AsInt64();
            Create.Column("user").OnTable("group_history").AsInt64();
        }
    }
}