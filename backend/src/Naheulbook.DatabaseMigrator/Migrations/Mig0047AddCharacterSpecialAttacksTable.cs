using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(47)]
public class Mig0047AddCharacterSpecialAttacksTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("characters_special_attacks")
            .WithColumn("characterId").AsInt64().NotNullable().ForeignKey("characters", "id").PrimaryKey()
            .WithColumn("specialAttackId").AsGuid().NotNullable().ForeignKey("special_attacks", "id").PrimaryKey()
            .WithColumn("data").AsCustom("json").NotNullable()
            ;
    }
}
