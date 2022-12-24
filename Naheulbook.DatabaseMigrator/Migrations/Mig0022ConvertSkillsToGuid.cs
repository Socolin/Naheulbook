using System;
using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(22)]
    public class Mig0022ConvertSkillsToGuid : Migration
    {
        public override void Up()
        {
            var convertedTableName = "skills";
            Alter.Table(convertedTableName).AddColumn("tmpBaseUuid").AsCustom("CHAR(36) CHARACTER SET ascii NOT NULL").SetExistingRowsTo(SystemMethods.NewGuid);
            Execute.WithConnection((connection, _) =>
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT @i:=uuid(); UPDATE `" + convertedTableName + "` SET tmpBaseUuid = (@i:=uuid());";
                command.ExecuteScalar();
            });

            Create.Index("IX_" + convertedTableName + "_id").OnTable(convertedTableName).OnColumn("tmpBaseUuid");

            ConvertSkillsLinkedTableToUuid(
                "origin_skills",
                "skillid",
                "FK_origin_skill_skill_skillid",
                "IX_origin_skill_skillid"
            );
            ConvertSkillsLinkedTableToUuid(
                "job_skills",
                "skillid",
                "FK_job_skill_skill_skillid",
                "IX_job_skill_skillid"
            );
            ConvertSkillsLinkedTableToUuid(
                "skill_effects",
                "skill",
                "FK_skill_effect_skill_skill",
                "IX_skill_effect_skill"
            );
            ConvertSkillsLinkedTableToUuid(
                "character_skills",
                "skill",
                "FK_character_skills_skill_skill",
                "IX_character_skills_skill"
            );
            ConvertSkillsLinkedTableToUuid(
                "item_template_skill_modifiers",
                "skill",
                "FK_item_skill_modifiers_skill_skill",
                "IX_item_skill_modifiers_skill"
            );
            ConvertSkillsLinkedTableToUuid(
                "item_template_unskills",
                "skill",
                "FK_item_unskill_skill_skill",
                "IX_item_unskill_skill"
            );
            ConvertSkillsLinkedTableToUuid(
                "item_template_skills",
                "skill",
                "FK_item_skill_skill_skill",
                "IX_item_skill_skill"
            );

            Delete.Column("id").FromTable(convertedTableName);
            Rename.Column("tmpBaseUuid").OnTable(convertedTableName).To("id");
            Create.PrimaryKey().OnTable(convertedTableName).Column("id");
        }

        private void ConvertSkillsLinkedTableToUuid(
            string tableName,
            string columnName,
            string foreignKey,
            string previousIndexName
        )
        {
            ConvertLinkedTableToUuid(tableName, foreignKey, columnName, "skillId", "skills", "id", previousIndexName);
        }

        private void ConvertLinkedTableToUuid(
            string tableName,
            string foreignKey,
            string columnName,
            string newColumnName,
            string targetTableName,
            string targetColumnName,
            string previousIndexName
        )
        {
            Alter.Table(tableName).AddColumn("tmpUuid").AsCustom("CHAR(36) CHARACTER SET ascii NOT NULL");
            Execute.Sql($"UPDATE `{tableName}` SET `tmpUuid` = (SELECT `tmpBaseUuid` FROM `{targetTableName}` WHERE `{columnName}` = {targetColumnName})");

            Delete.ForeignKey(foreignKey).OnTable(tableName);

            Alter.Column(columnName).OnTable(tableName).AsCustom("CHAR(36) CHARACTER SET ascii NOT NULL");
            Execute.Sql($"UPDATE `{tableName}` SET `{columnName}` = `tmpUuid`");
            Delete.Column("tmpUuid").FromTable(tableName);

            Rename.Column(columnName).OnTable(tableName).To(newColumnName);
            Alter.Column(newColumnName).OnTable(tableName).AsCustom("CHAR(36) CHARACTER SET ascii NOT NULL");

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
}