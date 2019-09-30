using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(10)]
    public class Mig0010MergeOriginColumnsToData : Migration
    {
        public override void Up()
        {
            Alter.Table("origin").AddColumn("data").AsCustom("json").NotNullable();

            Execute.Sql("UPDATE `origin` SET `data` = JSON_OBJECT()");
            Execute.Sql("UPDATE `origin` SET `data` = JSON_INSERT(`data`, '$.baseEv', baseEv) WHERE baseEv IS NOT NULL;");
            Execute.Sql("UPDATE `origin` SET `data` = JSON_INSERT(`data`, '$.bonusAt', bonusAt) WHERE bonusAt IS NOT NULL;");
            Execute.Sql("UPDATE `origin` SET `data` = JSON_INSERT(`data`, '$.bonusPrd', bonusPrd) WHERE bonusPrd IS NOT NULL;");
            Execute.Sql("UPDATE `origin` SET `data` = JSON_INSERT(`data`, '$.diceEvLevelUp', diceEvLevelUp) WHERE diceEvLevelUp IS NOT NULL;");
            Execute.Sql("UPDATE `origin` SET `data` = JSON_INSERT(`data`, '$.maxArmorPr', maxArmorPr) WHERE maxLoad IS NOT NULL;");
            Execute.Sql("UPDATE `origin` SET `data` = JSON_INSERT(`data`, '$.maxLoad', maxLoad) WHERE maxLoad IS NOT NULL;");
            Execute.Sql("UPDATE `origin` SET `data` = JSON_INSERT(`data`, '$.speedModifier', speedModifier) WHERE speedModifier IS NOT NULL;");

            Delete.Column("baseEa").FromTable("origin");
            Delete.Column("baseEv").FromTable("origin");
            Delete.Column("bonusAt").FromTable("origin");
            Delete.Column("bonusPrd").FromTable("origin");
            Delete.Column("diceEvLevelUp").FromTable("origin");
            Delete.Column("maxArmorPr").FromTable("origin");
            Delete.Column("maxLoad").FromTable("origin");
            Delete.Column("speedModifier").FromTable("origin");
        }

        public override void Down()
        {
            Delete.Column("data").FromTable("origin");

            Alter.Table("origin").AddColumn("baseEa").AsInt16().Nullable();
            Alter.Table("origin").AddColumn("baseEv").AsInt16().Nullable();
            Alter.Table("origin").AddColumn("bonusAt").AsInt16().Nullable();
            Alter.Table("origin").AddColumn("bonusPrd").AsInt16().Nullable();
            Alter.Table("origin").AddColumn("diceEvLevelUp").AsInt16().Nullable();
            Alter.Table("origin").AddColumn("maxArmorPr").AsInt16().Nullable();
            Alter.Table("origin").AddColumn("maxLoad").AsInt16().Nullable();
            Alter.Table("origin").AddColumn("speedModifier").AsInt16().Nullable();
        }
    }
}