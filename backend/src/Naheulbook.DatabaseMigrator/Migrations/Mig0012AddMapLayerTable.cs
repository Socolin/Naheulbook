using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(12)]
public class Mig0012AddMapLayerTable : Migration
{
    public override void Up()
    {
        Create.Table("map_layers")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("mapId").AsInt32().ForeignKey("fk_map_layer_map", "map", "id")
            .WithColumn("name").AsString(255)
            .WithColumn("source").AsString(25)
            .WithColumn("userId").AsInt64().Nullable().ForeignKey("fk_map_layer_user", "user", "id").OnDelete(Rule.Cascade)
            ;
    }

    public override void Down()
    {
        Delete.Table("map_layers");
    }
}