using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(38)]
public class Mig0038AddFightsTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("fights")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("groupid").AsInt64().ForeignKey("FK_fights_groupId_groups_id", "groups", "id")
            .OnDelete(Rule.Cascade);
    }
}