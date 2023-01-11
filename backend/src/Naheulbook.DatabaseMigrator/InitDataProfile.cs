using FluentMigrator;

namespace Naheulbook.DatabaseMigrator;

[Profile("InitialData")]
public class InitDataProfile : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("init_data.sql");
    }
    public override void Down()
    {
    }
}