using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(9)]
public class Mig0009MergeJobColumnsToData : Migration
{
    public override void Up()
    {
        Alter.Table("job").AddColumn("data").AsCustom("json").NotNullable();
        Execute.Sql("DELETE FROM `job` WHERE parentJob IS NOT NULL");
        Execute.Sql("UPDATE job SET data = JSON_OBJECT()");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin', JSON_OBJECT());");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all', JSON_OBJECT());");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.baseAt', baseAt) WHERE baseAt IS NOT NULL;");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.baseEa', baseEa) WHERE baseEa IS NOT NULL;");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.basePrd', basePrd) WHERE basePrd IS NOT NULL;");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.bonusEv', bonusEv) WHERE bonusEv IS NOT NULL;");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.factorEv', factorEv) WHERE factorEv IS NOT NULL;");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.maxArmorPr', maxArmorPr) WHERE maxArmorPr IS NOT NULL;");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.diceEaLevelUp', diceEaLevelUp) WHERE diceEaLevelUp IS NOT NULL;");
        Execute.Sql("UPDATE job SET data = JSON_INSERT(data, '$.forOrigin.all.maxLoad', maxLoad) WHERE maxLoad IS NOT NULL;");

        Delete.ForeignKey("FK_job_job_parentjob").OnTable("job");
        Delete.Column("baseAt").FromTable("job");
        Delete.Column("baseEa").FromTable("job");
        Delete.Column("basePrd").FromTable("job");
        Delete.Column("bonusEv").FromTable("job");
        Delete.Column("factorEv").FromTable("job");
        Delete.Column("maxArmorPr").FromTable("job");
        Delete.Column("maxLoad").FromTable("job");
        Delete.Column("baseEv").FromTable("job");
        Delete.Column("diceEaLevelUp").FromTable("job");
        Delete.Column("parentJob").FromTable("job");
        Delete.Column("internalname").FromTable("job");

        Alter.Column("name").OnTable("job").AsString(255);
    }

    public override void Down()
    {
        Delete.Column("data").FromTable("job");

        Alter.Table("job").AddColumn("baseAt").AsInt32().Nullable();
        Alter.Table("job").AddColumn("baseEa").AsInt32().Nullable();
        Alter.Table("job").AddColumn("basePrd").AsInt32().Nullable();
        Alter.Table("job").AddColumn("bonusEv").AsInt32().Nullable();
        Alter.Table("job").AddColumn("factorEv").AsInt32().Nullable();
        Alter.Table("job").AddColumn("maxArmorPr").AsInt32().Nullable();
        Alter.Table("job").AddColumn("baseEv").AsInt32().Nullable();
        Alter.Table("job").AddColumn("diceEaLevelUp").AsInt32().Nullable();
        Alter.Table("job").AddColumn("parentJob").AsInt32().Nullable();
    }
}