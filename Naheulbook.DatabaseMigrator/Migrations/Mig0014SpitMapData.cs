using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(13)]
    public class Mig0013SpitMapData : Migration
    {
        public override void Up()
        {
            Rename.Table("map").To("maps");
            Alter.Table("maps").AddColumn("imageData").AsCustom("json").NotNullable().SetExistingRowsTo("{}");
            Execute.Sql("UPDATE maps SET imageData = JSON_INSERT(imageData, '$.width', JSON_EXTRACT(data, '$.width'))");
            Execute.Sql("UPDATE maps SET imageData = JSON_INSERT(imageData, '$.height', JSON_EXTRACT(data, '$.height'))");
            Execute.Sql("UPDATE maps SET imageData = JSON_INSERT(imageData, '$.zoomCount', JSON_EXTRACT(data, '$.zoomCount'))");
            Execute.Sql("UPDATE maps SET imageData = JSON_INSERT(imageData, '$.extraZoomCount', JSON_EXTRACT(data, '$.extraZoomCount'))");
        }

        public override void Down()
        {
            Delete.Column("imageData").FromTable("maps");
            Rename.Table("maps").To("map");
        }
    }
}