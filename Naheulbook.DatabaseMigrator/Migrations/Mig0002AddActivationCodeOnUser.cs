using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(2)]
    public class Mig0002AddActivationCodeOnUser : Migration
    {
        public override void Up()
        {
            Alter.Table("user").AddColumn("activationCode").AsString(128).Nullable();
        }

        public override void Down()
        {
            Delete.Column("activationCode").FromTable("user");
        }
    }
}