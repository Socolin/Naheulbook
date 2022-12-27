using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(19)]
public class Mig0019RenameIndexesSubCategory : Migration
{
    public override void Up()
    {
        Execute.Sql("ALTER TABLE effect RENAME INDEX `IX_effect_category` TO `IX_effect_subCategoryId`");
        Execute.Sql("ALTER TABLE item_template RENAME INDEX `IX_item_template_category` TO `IX_item_template_subCategoryId`");
        Execute.Sql("ALTER TABLE monster_template RENAME INDEX `IX_monster_template_type` TO `IX_monster_template_subCategoryId`");
    }

    public override void Down()
    {
        Execute.Sql("ALTER TABLE effect RENAME INDEX `IX_effect_subCategoryId` TO `IX_effect_category`");
        Execute.Sql("ALTER TABLE item_template RENAME INDEX `IX_item_template_subCategoryId` TO `IX_item_template_category`");
        Execute.Sql("ALTER TABLE monster_template RENAME INDEX `IX_monster_template_subCategoryId` TO `IX_monster_template_type`");
    }
}