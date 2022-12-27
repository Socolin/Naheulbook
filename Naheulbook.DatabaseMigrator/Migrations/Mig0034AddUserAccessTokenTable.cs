using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(34)]
public class Mig0034AddUserAccessTokenTable : Migration
{
    private const string TableName = "user_access_token";

    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("userId").AsInt64().ForeignKey("FK_user_access_token_users", "users", "id")
            .WithColumn("name").AsString(255)
            .WithColumn("dateCreated").AsDateTime2()
            .WithColumn("key").AsString(255).Indexed();
    }

    public override void Down()
    {
        Delete.Table(TableName);
    }
}