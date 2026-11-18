using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

/// <summary>
/// Hybrids can choose which aptitude (APE) table they use: store the choice on the character
/// </summary>
[Migration(48)]
public class Mig0048AddAptitudeGroupIdOnCharacter : AutoReversingMigration
{
    public override void Up()
    {
        Alter.Table("characters")
            .AddColumn("aptitudeGroupId").AsGuid().Nullable().ForeignKey("aptitude_groups", "id");
    }
}
