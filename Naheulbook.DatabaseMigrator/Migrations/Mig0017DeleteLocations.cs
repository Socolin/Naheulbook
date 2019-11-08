using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(17)]
    public class Mig0017DeleteLocations : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_group_location_location").OnTable("group");
            Delete.Column("location").FromTable("group");
            Delete.Table("monster_location");
            Delete.ForeignKey("FK_location_map_location_location").OnTable("location_map");
            Delete.Table("location_map");
            Delete.Table("location");
        }

        public override void Down()
        {
            Alter.Table("group").AddColumn("location").AsInt32().Nullable();
            Create.Table("location")
                .WithColumn("id").AsInt32().PrimaryKey().Identity();
            Create.Table("location_map")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("locationId").AsInt32().Nullable().Indexed();
            Create.Table("monster_location").WithColumn("id").AsInt32();

            Create.ForeignKey("FK_location_map_location_location")
                .FromTable("location_map").ForeignColumn("locationId")
                .ToTable("location").PrimaryColumn("id");
            Create.ForeignKey("FK_group_location_location")
                .FromTable("group").ForeignColumn("location")
                .ToTable("location").PrimaryColumn("id");
        }
    }
}