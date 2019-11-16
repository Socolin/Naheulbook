using System;
using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(25)]
    public class Mig0025ConvertSpecialitiesToGuid : Migration
    {
        public override void Up()
        {
            var convertedTableName = "specialities";
            Alter.Table(convertedTableName).AddColumn("tmpBaseUuid").AsCustom("CHAR(36) CHARACTER SET ascii NOT NULL").SetExistingRowsTo(SystemMethods.NewGuid);
            Execute.WithConnection((connection, transaction) =>
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT @i:=uuid(); UPDATE `" + convertedTableName + "` SET tmpBaseUuid = (@i:=uuid());";
                command.ExecuteScalar();
            });

            Create.Index("IX_" + convertedTableName + "_id").OnTable(convertedTableName).OnColumn("tmpBaseUuid");

            ConvertSpecialitiesLinkedTableToUuid(
                "speciality_modifiers",
                "speciality",
                "FK_speciality_modifier_speciality_speciality",
                "IX_speciality_modifier_speciality"
            );
            ConvertSpecialitiesLinkedTableToUuid(
                "speciality_specials",
                "speciality",
                "FK_speciality_special_speciality_speciality",
                "IX_speciality_special_speciality"
            );
            ConvertSpecialitiesLinkedTableToUuid(
                "character_specialities",
                "speciality",
                "FK_character_speciality_speciality_speciality",
                "IX_character_speciality_speciality"
            );

            Delete.Column("id").FromTable(convertedTableName);
            Rename.Column("tmpBaseUuid").OnTable(convertedTableName).To("id");
            Create.PrimaryKey().OnTable(convertedTableName).Column("id");
        }

        private void ConvertSpecialitiesLinkedTableToUuid(
            string tableName,
            string columnName,
            string foreignKey,
            string previousIndexName = null
        )
        {
            ConvertLinkedTableToUuid(tableName, foreignKey, columnName, "specialityId", "specialities", "id", previousIndexName);
        }

        private void ConvertLinkedTableToUuid(
            string tableName,
            string foreignKey,
            string columnName,
            string newColumnName,
            string targetTableName,
            string targetColumnName,
            string previousIndexName,
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
}