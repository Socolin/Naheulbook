using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(41)]
public class Mig0041AddAptitudeTables : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("aptitude_groups")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            ;

        Create.Table("aptitudes")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("aptitudeGroupId").AsGuid().ForeignKey("aptitude_groups", "id")
            .WithColumn("roll").AsInt32().NotNullable()
            .WithColumn("type").AsString(255).NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).NotNullable()
            .WithColumn("effect").AsString(int.MaxValue).NotNullable()
            ;

        Create.UniqueConstraint()
            .OnTable("aptitudes")
            .Columns("aptitudeGroupId", "roll");

        Alter.Table("origins")
            .AddColumn("aptitudeGroupId").AsGuid().Nullable().ForeignKey("aptitude_groups", "id")
            ;

        Create.Table("characters_aptitudes")
            .WithColumn("aptitudeId").AsGuid().NotNullable().ForeignKey("aptitudes", "id").PrimaryKey()
            .WithColumn("characterId").AsInt64().ForeignKey("characters", "id").PrimaryKey()
            ;
    }
}