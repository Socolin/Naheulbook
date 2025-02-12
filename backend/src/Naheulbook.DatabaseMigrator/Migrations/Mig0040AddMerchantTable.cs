using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(40)]
public class Mig0040AddMerchantTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("merchants")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("groupid").AsInt64().ForeignKey("FK_merchants_groupId_groups_id", "groups", "id")
            .OnDelete(Rule.Cascade);
    }
}