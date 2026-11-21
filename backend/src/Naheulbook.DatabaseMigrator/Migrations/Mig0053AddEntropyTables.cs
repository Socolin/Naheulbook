using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

/// <summary>
/// D100 tables of the "pack de l'entropie": calamities drawn when the group reaches 5 Mankdebol points,
/// blessings drawn when it reaches 5 Débilibeuk points.
/// </summary>
[Migration(53)]
public class Mig0053AddEntropyTables : AutoReversingMigration
{
    public override void Up()
    {
        // `diceRoll` is not unique: the official calamity table has two entries on 44
        Create.Table("mankdebol_calamities")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("diceRoll").AsInt32().NotNullable().Indexed()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("effect").AsString(int.MaxValue).NotNullable()
            ;

        Create.Table("debilibeuk_blessings")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("diceRoll").AsInt32().NotNullable().Indexed()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("effect").AsString(int.MaxValue).NotNullable()
            ;
    }
}
