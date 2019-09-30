using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(8)]
    public class Mig0008RemoveJobBlacklistWhitelist : Migration
    {
        public override void Up()
        {
            Delete.Table("job_origin_blacklist");
            Delete.Table("job_origin_whitelist");
            Execute.Sql(@"UPDATE `character_job` 
                INNER JOIN job ON
                    job.id = character_job.job
                SET job = job.parentjob 
                    WHERE parentjob IS NOT NULL
                ");
            Execute.Sql("DELETE FROM job WHERE parentjob IS NOT NULL");
        }

        public override void Down()
        {
        }
    }
}