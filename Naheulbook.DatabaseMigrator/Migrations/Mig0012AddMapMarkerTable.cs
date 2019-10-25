using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(13)]
    public class Mig0013AddMapMarkerTable : Migration
    {
        public override void Up()
        {
            Create.Table("map_markers")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("layerId").AsInt32().ForeignKey("fk_map_markers_map_layers", "map_layers", "id")
                .WithColumn("name").AsString(255)
                .WithColumn("type").AsString(25)
                .WithColumn("description").AsString(10000).Nullable()
                .WithColumn("markerInfo").AsCustom("json")
                ;
        }

        public override void Down()
        {
            Delete.Table("map_markers");
        }
    }
}