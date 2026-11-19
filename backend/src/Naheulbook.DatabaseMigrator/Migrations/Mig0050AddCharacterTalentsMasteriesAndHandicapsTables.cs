using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(50)]
public class Mig0050AddCharacterTalentsMasteriesAndHandicapsTables : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("characters_talents")
            .WithColumn("characterId").AsInt64().NotNullable().ForeignKey("characters", "id").PrimaryKey()
            .WithColumn("talentId").AsGuid().NotNullable().ForeignKey("talents", "id").PrimaryKey()
            ;

        Create.Table("characters_masteries")
            .WithColumn("characterId").AsInt64().NotNullable().ForeignKey("characters", "id").PrimaryKey()
            .WithColumn("masteryId").AsGuid().NotNullable().ForeignKey("masteries", "id").PrimaryKey()
            ;

        Create.Table("characters_handicaps")
            .WithColumn("characterId").AsInt64().NotNullable().ForeignKey("characters", "id").PrimaryKey()
            .WithColumn("handicapId").AsGuid().NotNullable().ForeignKey("handicaps", "id").PrimaryKey()
            ;
    }
}
