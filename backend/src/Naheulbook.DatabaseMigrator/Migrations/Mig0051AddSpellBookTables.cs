using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

/// <summary>
/// Spell books (mage grimoires, priest and paladin manuals) and their spells / prodiges / rituals / invocations.
/// The legacy `spells` and `spell_categories` tables were never used, so they are replaced.
/// </summary>
[Migration(51)]
public class Mig0051AddSpellBookTables : Migration
{
    private const string AsciiGuid = "char(36) CHARACTER SET ascii COLLATE ascii_general_ci";

    public override void Up()
    {
        Delete.Table("spells");
        Delete.Table("spell_categories");

        Create.Table("spell_books")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("techName").AsString(64).NotNullable().Unique()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("kind").AsString(32).NotNullable()
            // `jobs.id` and `specialities.id` are ascii char(36): the foreign key needs the same charset
            .WithColumn("jobId").AsCustom(AsciiGuid).Nullable().ForeignKey("jobs", "id")
            .WithColumn("specialityId").AsCustom(AsciiGuid).Nullable().ForeignKey("specialities", "id")
            .WithColumn("godId").AsInt32().Nullable().ForeignKey("gods", "id")
            .WithColumn("displayOrder").AsInt32().NotNullable()
            .WithColumn("data").AsCustom("json").NotNullable()
            ;

        Create.Table("spells")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("spellBookId").AsGuid().NotNullable().ForeignKey("spell_books", "id")
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("level").AsInt32().Nullable()
            .WithColumn("levelLabel").AsString(32).Nullable()
            .WithColumn("kind").AsString(32).NotNullable()
            .WithColumn("category").AsString(255).Nullable()
            .WithColumn("displayOrder").AsInt32().NotNullable()
            .WithColumn("description").AsString(int.MaxValue).NotNullable()
            .WithColumn("data").AsCustom("json").NotNullable()
            ;
    }

    public override void Down()
    {
        Delete.Table("spells");
        Delete.Table("spell_books");

        Create.Table("spell_categories")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("default").AsCustom("bit(1)").NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            ;

        Create.Table("spells")
            .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
            .WithColumn("category").AsInt64().NotNullable().ForeignKey("FK_spell_spell_category_category", "spell_categories", "id")
            .WithColumn("cost").AsInt64().NotNullable()
            .WithColumn("description").AsCustom("longtext").NotNullable()
            .WithColumn("distance").AsInt64().Nullable()
            .WithColumn("distancenote").AsCustom("longtext").Nullable()
            .WithColumn("durationeffect").AsInt64().NotNullable()
            .WithColumn("durationeffectunit").AsString(255).NotNullable()
            .WithColumn("durationprepare").AsInt64().NotNullable()
            .WithColumn("durationprepareunit").AsString(255).NotNullable()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("speed").AsInt64().NotNullable()
            .WithColumn("test").AsInt64().NotNullable()
            .WithColumn("teststat").AsString(255).NotNullable().ForeignKey("FK_spell_stat_teststat", "stats", "name")
            .WithColumn("type").AsString(255).NotNullable()
            .WithColumn("variablecost").AsCustom("bit(1)").NotNullable()
            ;
    }
}
