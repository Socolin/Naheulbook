using System;
using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(28)]
public class Mig0028ConvertItemTemplateToGuid : Migration
{
    public override void Up()
    {
        var convertedTableName = "item_templates";
        Alter.Table(convertedTableName).AddColumn("tmpBaseUuid").AsCustom("CHAR(36) CHARACTER SET ascii NOT NULL").SetExistingRowsTo(SystemMethods.NewGuid);
        Execute.WithConnection((connection, _) =>
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT @i:=uuid(); UPDATE `" + convertedTableName + "` SET tmpBaseUuid = (@i:=uuid());";
            command.ExecuteScalar();
        });

        Create.Index("IX_" + convertedTableName + "_id").OnTable(convertedTableName).OnColumn("tmpBaseUuid");

        ConvertItemTemplatesLinkedTableToUuid(
            "item_template_modifiers",
            "itemTemplate",
            "FK_item_effect_item_template_item",
            "IX_item_effect_item"
        );
        ConvertItemTemplatesLinkedTableToUuid(
            "item_template_requirements",
            "itemTemplate",
            "FK_item_requirement_item_template_item",
            "IX_item_requirement_item"
        );
        ConvertItemTemplatesLinkedTableToUuid(
            "item_template_skill_modifiers",
            "itemTemplate",
            "FK_item_skill_modifiers_item_template_item",
            "IX_item_skill_modifiers_item"
        );
        ConvertItemTemplatesLinkedTableToUuid(
            "item_template_skills",
            "itemTemplate",
            "FK_item_skill_item_template_item",
            "IX_item_skill_item"
        );
        ConvertItemTemplatesLinkedTableToUuid(
            "item_template_slots",
            "itemTemplate",
            "FK_item_template_slot_item_template_item",
            "IX_item_template_slot_item"
        );
        ConvertItemTemplatesLinkedTableToUuid(
            "item_template_unskills",
            "itemTemplate",
            "FK_item_unskill_item_template_item",
            "IX_item_unskill_item"
        );
        ConvertItemTemplatesLinkedTableToUuid(
            "items",
            "itemtemplateid",
            "FK_item_item_template_itemtemplateid",
            "IX_item_itemtemplateid"
        );

        ConvertItemTemplatesLinkedTableToUuid(
            "monster_template_inventory_elements",
            "itemtemplateid",
            "FK_monster_template_simple_inventory_item_template_itemtemplatei",
            "IX_monster_template_simple_inventory_itemtemplateid"
        );

        Execute.EmbeddedScript(@"Mig0028UpdateItemTemplateActionsItemTemplateIdReferences.sql");

        Delete.Column("id").FromTable(convertedTableName);
        Rename.Column("tmpBaseUuid").OnTable(convertedTableName).To("id");
        Create.PrimaryKey().OnTable(convertedTableName).Column("id");
    }

    private void ConvertItemTemplatesLinkedTableToUuid(
        string tableName,
        string columnName,
        string foreignKey,
        string? previousIndexName = null
    )
    {
        ConvertLinkedTableToUuid(tableName, foreignKey, columnName, "itemTemplateId", "item_templates", "id", previousIndexName);
    }

    private void ConvertLinkedTableToUuid(
        string tableName,
        string foreignKey,
        string columnName,
        string newColumnName,
        string targetTableName,
        string targetColumnName,
        string? previousIndexName,
        bool nullable = false
    )
    {
        Alter.Table(tableName).AddColumn("tmpUuid").AsCustom("CHAR(36) CHARACTER SET ascii").Nullable();
        Execute.Sql($"UPDATE `{tableName}` SET `tmpUuid` = (SELECT `tmpBaseUuid` FROM `{targetTableName}` WHERE `{tableName}`.`{columnName}` = `{targetTableName}`.`{targetColumnName}`)");

        Delete.ForeignKey(foreignKey).OnTable(tableName);

        Alter.Column(columnName).OnTable(tableName).AsCustom("CHAR(36) CHARACTER SET ascii").Nullable();
        Execute.Sql($"UPDATE `{tableName}` SET `{columnName}` = `tmpUuid`");
        Delete.Column("tmpUuid").FromTable(tableName);

        Rename.Column(columnName).OnTable(tableName).To(newColumnName);
        if (nullable)
            Alter.Column(newColumnName).OnTable(tableName).AsCustom("CHAR(36) CHARACTER SET ascii").Nullable();
        else
            Alter.Column(newColumnName).OnTable(tableName).AsCustom("CHAR(36) CHARACTER SET ascii").NotNullable();

        if (!string.IsNullOrEmpty(previousIndexName))
            Delete.Index(previousIndexName).OnTable(tableName);
        Create.Index("IX_" + tableName + "_" + newColumnName).OnTable(tableName).OnColumn(newColumnName);
        var foreignKeyName = "FK_" + tableName + "_" + newColumnName + "_" + targetTableName + "_" + targetColumnName;
        if (foreignKeyName.Length > 64)
        {
            foreignKeyName = "FK_" + tableName + "_" + newColumnName;
        }

        Create.ForeignKey(foreignKeyName)
            .FromTable(tableName).ForeignColumn(newColumnName)
            .ToTable(targetTableName).PrimaryColumn("tmpBaseUuid")
            .OnDeleteOrUpdate(Rule.None);
    }

    public override void Down()
    {
        throw new NotSupportedException();
    }
}