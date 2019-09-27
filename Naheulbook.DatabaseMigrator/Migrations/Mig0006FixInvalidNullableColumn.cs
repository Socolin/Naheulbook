using System;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(6)]
    public class Mig0006FixInvalidNullableColumn : Migration
    {
        public override void Up()
        {
            Alter.Column("character").OnTable("character_job").AsInt64().NotNullable();
            Alter.Column("job").OnTable("character_job").AsInt64().NotNullable();
            Alter.Column("order").OnTable("character_job").AsInt64().NotNullable();

            Execute.Sql("UPDATE character_modifier SET `durationtype` = 'forever' WHERE durationtype IS NULL");
            Alter.Column("durationtype").OnTable("character_modifier").AsString(10).NotNullable();

            Alter.Column("typeid").OnTable("effect_category").AsInt64().NotNullable();

            Execute.Sql("UPDATE effect SET `durationtype` = 'forever' WHERE durationtype IS NULL");
            Alter.Column("durationtype").OnTable("effect").AsString(10).NotNullable();

            Alter.Column("techname").OnTable("god").AsString(32).NotNullable();
        }

        public override void Down()
        {
            Alter.Column("character").OnTable("character_job").AsInt64().Nullable();
            Alter.Column("job").OnTable("character_job").AsInt64().Nullable();
            Alter.Column("order").OnTable("character_job").AsInt64().Nullable();

            Alter.Column("durationtype").OnTable("character_modifier").AsString(255).Nullable();

            Alter.Column("typeid").OnTable("effect_category").AsInt64().Nullable();

            Alter.Column("durationtype").OnTable("effect").AsString(255).Nullable();

            Alter.Column("techname").OnTable("god").AsString(255).Nullable();
        }
    }
}