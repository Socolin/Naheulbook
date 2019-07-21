using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(5)]
    public class Mig0005AddOriginRandomNameGeneratorUrlTable : Migration
    {
        public override void Up()
        {
            Create.Table("origin_random_name_urls")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("originId").AsInt64()
                .WithColumn("sex").AsString(255)
                .WithColumn("url").AsString(255);
            Create.ForeignKey()
                .FromTable("origin_random_name_urls").ForeignColumn("originId").ToTable("origin").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.Table("origin_random_name_urls");
        }
    }
}