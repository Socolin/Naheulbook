using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations
{
    [Migration(1)]
    public class Mig0001Init : Migration
    {
        public override void Up()
        {
            Execute.EmbeddedScript("init.sql");
        }

        public override void Down()
        {
            var tableNames = new[]
            {
                "calendar", "character", "character_history", "character_job", "character_modifier",
                "character_modifier_value", "character_skills", "character_speciality", "effect", "effect_category",
                "effect_modifier", "effect_type", "error_report", "event", "god", "group", "group_history",
                "group_invitations", "icon", "item", "item_slot", "item_template", "item_template_category",
                "item_template_modifier", "item_template_requirement", "item_template_section", "item_template_skill",
                "item_template_skill_modifiers", "item_template_slot", "item_template_unskill", "item_type", "job",
                "job_bonus", "job_origin_blacklist", "job_origin_whitelist", "job_requirement", "job_restrict",
                "job_skill", "location", "location_map", "loot", "monster", "monster_category", "monster_location",
                "monster_template", "monster_template_simple_inventory", "monster_trait", "monster_type", "origin",
                "origin_bonus", "origin_info", "origin_requirement", "origin_restrict", "origin_skill", "quest",
                "quest_template", "skill", "skill_effect", "speciality", "speciality_modifier", "speciality_special",
                "spell", "spell_category", "stat", "user", "user_session"
            };


            Execute.Sql("SET FOREIGN_KEY_CHECKS=0;");
            foreach (var tableName in tableNames)
            {
                Delete.Table(tableName);
            }
            Execute.Sql("SET FOREIGN_KEY_CHECKS=1;");
        }
    }
}