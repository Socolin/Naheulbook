using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(46)]
public class Mig0046AddSpecialAttackTables : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("special_attack_groups")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("displayOrder").AsInt32().NotNullable()
            ;

        Create.Table("special_attacks")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("specialAttackGroupId").AsGuid().NotNullable().ForeignKey("special_attack_groups", "id")
            .WithColumn("displayOrder").AsInt32().NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("data").AsCustom("json").NotNullable()
            ;
    }
}
