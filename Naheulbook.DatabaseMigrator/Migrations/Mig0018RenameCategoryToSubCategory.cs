using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(18)]
public class Mig0018RenameCategoryToSubCategory : AutoReversingMigration
{
    public override void Up()
    {
        Rename.Table("item_template_category").To("item_template_subcategories");
        Rename.Column("category").OnTable("item_template").To("subCategoryId");

        Rename.Table("effect_category").To("effect_subcategories");
        Rename.Column("category").OnTable("effect").To("subCategoryId");

        Rename.Table("monster_category").To("monster_subcategories");
        Rename.Column("categoryid").OnTable("monster_template").To("subCategoryId");
    }
}