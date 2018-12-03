using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(3)]
    public class Mig0003EffectTypeIdAsAutoincrement : Migration
    {
        public override void Up()
        {
            Execute.Sql("SET FOREIGN_KEY_CHECKS=0;");
            Alter.Table("effect_type").AlterColumn("id").AsInt64().PrimaryKey().Identity();
            Execute.Sql("SET FOREIGN_KEY_CHECKS=1;");
        }

        public override void Down()
        {
            Execute.Sql("SET FOREIGN_KEY_CHECKS=0;");
            Alter.Table("effect_type").AlterColumn("id").AsInt64().PrimaryKey();
            Execute.Sql("SET FOREIGN_KEY_CHECKS=1;");
        }
    }
}