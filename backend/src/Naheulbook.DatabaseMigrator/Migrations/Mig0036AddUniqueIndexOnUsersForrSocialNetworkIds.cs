using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(36)]
public class Mig0036AddNotesColumnOnCharacters : Migration
{
    public override void Up()
    {
        Create.UniqueConstraint("IX_users_fbId").OnTable("users").Column("fbId");
        Create.UniqueConstraint("IX_users_googleId").OnTable("users").Column("googleId");
        Create.UniqueConstraint("IX_users_twitterId").OnTable("users").Column("twitterId");
        Create.UniqueConstraint("IX_users_liveId").OnTable("users").Column("liveId");
    }

    public override void Down()
    {
        Delete.UniqueConstraint("IX_users_fbId").FromTable("users");
        Delete.UniqueConstraint("IX_users_googleId").FromTable("users");
        Delete.UniqueConstraint("IX_users_twitterId").FromTable("users");
        Delete.UniqueConstraint("IX_users_liveId").FromTable("users");
    }
}