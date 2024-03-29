using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(20)]
public class Mig0020NormalizeTableNames : AutoReversingMigration
{
    public override void Up()
    {
        Rename.Table("calendar").To("calendars");
        Rename.Table("character").To("characters");
        Rename.Table("character_history").To("character_history_entries");
        Rename.Table("character_job").To("character_jobs");
        Rename.Table("character_modifier").To("character_modifiers");
        Rename.Table("character_modifier_value").To("character_modifier_values");
        Rename.Table("character_speciality").To("character_specialities");
        Rename.Table("effect").To("effects");
        Rename.Table("effect_modifier").To("effect_modifiers");
        Rename.Table("effect_type").To("effect_types");
        Rename.Table("event").To("events");
        Rename.Table("god").To("gods");
        Rename.Table("group").To("groups");
        Rename.Table("group_history").To("group_history_entries");
        Rename.Table("item").To("items");
        Rename.Table("item_slot").To("slots");
        Rename.Table("item_template").To("item_templates");
        Rename.Table("item_template_modifier").To("item_template_modifiers");
        Rename.Table("item_template_requirement").To("item_template_requirements");
        Rename.Table("item_template_section").To("item_template_sections");
        Rename.Table("item_template_skill").To("item_template_skills");
        Rename.Table("item_template_slot").To("item_template_slots");
        Rename.Table("item_template_unskill").To("item_template_unskills");
        Rename.Table("item_type").To("item_types");
        Rename.Table("job").To("jobs");
        Rename.Table("job_bonus").To("job_bonuses");
        Rename.Table("job_requirement").To("job_requirements");
        Rename.Table("job_restrict").To("job_restrictions");
        Rename.Table("job_skill").To("job_skills");
        Rename.Table("loot").To("loots");
        Rename.Table("monster").To("monsters");
        Rename.Table("monster_template").To("monster_templates");
        Rename.Table("monster_template_simple_inventory").To("monster_template_inventory_elements");
        Rename.Table("monster_trait").To("monster_traits");
        Rename.Table("monster_type").To("monster_types");
        Rename.Table("origin").To("origins");
        Rename.Table("origin_bonus").To("origin_bonuses");
        Rename.Table("origin_info").To("origin_information");
        Rename.Table("origin_requirement").To("origin_requirements");
        Rename.Table("origin_restrict").To("origin_restrictions");
        Rename.Table("origin_skill").To("origin_skills");
        Rename.Table("quest").To("quests");
        Rename.Table("quest_template").To("quest_templates");
        Rename.Table("skill").To("skills");
        Rename.Table("skill_effect").To("skill_effects");
        Rename.Table("speciality").To("specialities");
        Rename.Table("speciality_modifier").To("speciality_modifiers");
        Rename.Table("speciality_special").To("speciality_specials");
        Rename.Table("spell").To("spells");
        Rename.Table("spell_category").To("spell_categories");
        Rename.Table("stat").To("stats");
        Rename.Table("user").To("users");
        Rename.Table("user_session").To("user_sessions");
    }
}