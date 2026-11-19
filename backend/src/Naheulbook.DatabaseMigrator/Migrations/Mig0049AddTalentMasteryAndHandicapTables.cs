using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(49)]
public class Mig0049AddTalentMasteryAndHandicapTables : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("talent_groups")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("displayOrder").AsInt32().NotNullable()
            ;

        Create.Table("talents")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("talentGroupId").AsGuid().NotNullable().ForeignKey("talent_groups", "id")
            .WithColumn("displayOrder").AsInt32().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("data").AsCustom("json").NotNullable()
            ;

        Create.Table("mastery_groups")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("displayOrder").AsInt32().NotNullable()
            ;

        Create.Table("masteries")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("masteryGroupId").AsGuid().NotNullable().ForeignKey("mastery_groups", "id")
            .WithColumn("displayOrder").AsInt32().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("data").AsCustom("json").NotNullable()
            ;

        Create.Table("handicaps")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("diceRoll").AsInt32().NotNullable().Unique()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("data").AsCustom("json").NotNullable()
            ;
    }
}
