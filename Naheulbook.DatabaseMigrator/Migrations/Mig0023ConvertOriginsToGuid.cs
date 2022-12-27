using System;
using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(23)]
public class Mig0023ConvertOriginsToGuid : Migration
{
    public override void Up()
    {
        var convertedTableName = "origins";
        Alter.Table(convertedTableName).AddColumn("tmpBaseUuid").AsCustom("CHAR(36) CHARACTER SET ascii NOT NULL").SetExistingRowsTo(SystemMethods.NewGuid);
        Execute.WithConnection((connection, _) =>
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT @i:=uuid(); UPDATE `" + convertedTableName + "` SET tmpBaseUuid = (@i:=uuid());";
            command.ExecuteScalar();
        });

        Create.Index("IX_" + convertedTableName + "_id").OnTable(convertedTableName).OnColumn("tmpBaseUuid");

        ConvertOriginsLinkedTableToUuid(
            "origin_skills",
            "originid",
            "FK_origin_skill_origin_originid"
        );
        ConvertOriginsLinkedTableToUuid(
            "origin_bonuses",
            "originid",
            "FK_origin_bonus_origin_originid",
            "IX_origin_bonus_originid"
        );
        ConvertOriginsLinkedTableToUuid(
            "origin_information",
            "originid",
            "FK_origin_info_origin_originid",
            "IX_origin_info_originid"
        );
        ConvertOriginsLinkedTableToUuid(
            "origin_random_name_urls",
            "originId",
            "FK_origin_random_name_urls_originId_origin_id"
        );
        ConvertOriginsLinkedTableToUuid(
            "origin_requirements",
            "originid",
            "FK_origin_requirement_origin_originid",
            "IX_origin_requirement_originid"
        );
        ConvertOriginsLinkedTableToUuid(
            "origin_restrictions",
            "originid",
            "FK_origin_restrict_origin_originid",
            "IX_origin_restrict_originid"
        );
        ConvertOriginsLinkedTableToUuid(
            "characters",
            "origin",
            "FK_character_origin_origin",
            "IX_character_origin"
        );
        ConvertLinkedTableToUuid(
            "item_template_modifiers",
            "FK_item_effect_origin_requireorigin",
            "requireorigin",
            "requiredOriginId",
            "origins",
            "id",
            "IX_item_effect_requireorigin",
            true
        );

        Delete.Column("id").FromTable(convertedTableName);
        Rename.Column("tmpBaseUuid").OnTable(convertedTableName).To("id");
        Create.PrimaryKey().OnTable(convertedTableName).Column("id");
    }

    private void ConvertOriginsLinkedTableToUuid(
        string tableName,
        string columnName,
        string foreignKey,
        string? previousIndexName = null
    )
    {
        ConvertLinkedTableToUuid(tableName, foreignKey, columnName, "originId", "origins", "id", previousIndexName);
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
        Create.ForeignKey("FK_" + tableName + "_" + newColumnName + "_" + targetTableName + "_" + targetColumnName)
            .FromTable(tableName).ForeignColumn(newColumnName)
            .ToTable(targetTableName).PrimaryColumn("tmpBaseUuid")
            .OnDeleteOrUpdate(Rule.None);
    }

    public override void Down()
    {
        throw new NotSupportedException();
    }
}