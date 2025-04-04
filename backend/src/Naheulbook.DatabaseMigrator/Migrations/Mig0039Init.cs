using System;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(39)]
public class Mig0039Init : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("init.sql");
    }

    public override void Down()
    {
        throw new NotSupportedException();
    }
}