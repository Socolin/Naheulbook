using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(37)]
public class Mig0037AddUniqueIndexOnRandomNameUrls : Migration
{
    public override void Up()
    {
        Create.UniqueConstraint("IX_origin_random_name_urls_originId_sec").OnTable("origin_random_name_urls").Columns("originId", "sex");
    }

    public override void Down()
    {
        Delete.UniqueConstraint("IX_origin_random_name_urls_originId_sec").FromTable("origin_random_name_urls");
    }
}