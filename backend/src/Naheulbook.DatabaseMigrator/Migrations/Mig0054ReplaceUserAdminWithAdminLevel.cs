using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

/// <summary>
/// Replace the boolean `admin` column of `users` by an `adminLevel`: 0 = none, 1 = admin, 2 = super admin.
/// Current admins become level 1.
/// </summary>
[Migration(54)]
public class Mig0054ReplaceUserAdminWithAdminLevel : Migration
{
    public override void Up()
    {
        Alter.Table("users")
            .AddColumn("adminLevel").AsInt32().NotNullable().WithDefaultValue(0);

        Execute.Sql("UPDATE `users` SET `adminLevel` = 1 WHERE `admin` = 1");

        Delete.Column("admin").FromTable("users");
    }

    public override void Down()
    {
        Alter.Table("users")
            .AddColumn("admin").AsBoolean().NotNullable().WithDefaultValue(false);

        Execute.Sql("UPDATE `users` SET `admin` = 1 WHERE `adminLevel` >= 1");

        Delete.Column("adminLevel").FromTable("users");
    }
}
