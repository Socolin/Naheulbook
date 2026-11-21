using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

/// <summary>
/// Spell books given to a character on top of the ones of its job and specialities (rare books, special events…),
/// optionally limited to the spells up to a given level.
/// </summary>
[Migration(52)]
public class Mig0052AddCharacterSpellBooksTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("characters_spell_books")
            .WithColumn("characterId").AsInt64().NotNullable().ForeignKey("characters", "id").PrimaryKey()
            .WithColumn("spellBookId").AsGuid().NotNullable().ForeignKey("spell_books", "id").PrimaryKey()
            .WithColumn("maxLevel").AsInt32().Nullable()
            ;
    }
}
