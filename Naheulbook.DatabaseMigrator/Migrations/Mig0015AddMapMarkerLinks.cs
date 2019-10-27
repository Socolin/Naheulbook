using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(15)]
    public class Mig0015AddMapMarkerLinks : Migration
    {
        public override void Up()
        {
            Create.Table("map_marker_links")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("mapMarkerId").AsInt32().ForeignKey("fk_map_markers_links_map_markers", "map_markers", "id")
                .WithColumn("name").AsString(255).Nullable()
                .WithColumn("targetMapId").AsInt32().ForeignKey("fk_map_markers_link_target_map", "maps", "id")
                .WithColumn("targetMapMarkerId").AsInt32().Nullable().ForeignKey("fk_map_markers_links_target_map_marker", "map_markers", "id")
                ;
        }

        public override void Down()
        {
            Delete.Table("map_marker_links");
        }
    }
}