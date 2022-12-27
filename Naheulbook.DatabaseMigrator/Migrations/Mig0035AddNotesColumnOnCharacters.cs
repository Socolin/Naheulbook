using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(35)]
public class Mig0035AddNotesColumnOnCharacters : Migration
{
    public override void Up()
    {
        Alter.Table("characters").AddColumn("notes").AsString(int.MaxValue).Nullable();
    }

    public override void Down()
    {
        Delete.Column("notes").FromTable("characters");
    }
}