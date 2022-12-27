using System;
using System.Data;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

[Migration(27)]
public class Mig0027NormalizeColumnName : Migration
{
    public override void Up()
    {
        Rename.Column("charactermodifier").OnTable("character_modifier_values").To("characterModifierId");
        CreateForeignKey(
            "IX_character_modifier_value_charactermodifier",
            "FK_character_modifier_value_character_modifier_charactermodifier",
            "character_modifier_values",
            "characterModifierId",
            "character_modifiers",
            "id",
            Rule.Cascade
        );

        Rename.Column("character").OnTable("character_modifiers").To("characterId");
        CreateForeignKey(
            "IX_character_modifier_character",
            "FK_character_modifier_character_character",
            "character_modifiers",
            "characterId",
            "characters",
            "id",
            Rule.Cascade
        );

        Rename.Column("character").OnTable("character_skills").To("characterId");
        CreateForeignKey(
            "IX_character_skills_character",
            "FK_character_skills_character_character",
            "character_skills",
            "characterId",
            "characters",
            "id",
            Rule.Cascade
        );

        Rename.Column("character").OnTable("character_specialities").To("characterId");
        CreateForeignKey(
            null,
            "FK_character_speciality_character_character",
            "character_specialities",
            "characterId",
            "characters",
            "id",
            Rule.Cascade
        );


        Rename.Column("group").OnTable("characters").To("groupId");
        CreateForeignKey(
            "IX_character_group",
            "FK_character_group_group",
            "characters",
            "groupId",
            "groups",
            "id",
            Rule.Cascade
        );
        Rename.Column("user").OnTable("characters").To("userId");
        CreateForeignKey(
            "IX_character_user",
            "FK_character_user_user",
            "characters",
            "userId",
            "users",
            "id",
            Rule.Cascade
        );


        Rename.Column("effect").OnTable("effect_modifiers").To("effectId");
        CreateForeignKey(
            null,
            "FK_effect_modifier_effect_effect",
            "effect_modifiers",
            "effectId",
            "effects",
            "id",
            Rule.Cascade
        );

        CreateForeignKey(
            "event_group_id_fk",
            "event_group_id_fk",
            "events",
            "groupId",
            "groups",
            "id",
            Rule.Cascade
        );



        Rename.Column("group").OnTable("group_invitations").To("groupId");
        CreateForeignKey(
            "IX_group_invitations_group",
            "FK_group_invitations_group_group",
            "group_invitations",
            "groupId",
            "groups",
            "id",
            Rule.Cascade
        );
        Rename.Column("character").OnTable("group_invitations").To("characterId");
        CreateForeignKey(
            "IX_group_invitations_character",
            "FK_group_invitations_character_character",
            "group_invitations",
            "characterId",
            "characters",
            "id",
            Rule.Cascade
        );

        Rename.Column("master").OnTable("groups").To("masterId");
        CreateForeignKey(
            "IX_group_master",
            "FK_group_user_master",
            "groups",
            "masterId",
            "users",
            "id",
            Rule.None
        );
    }


    private void CreateForeignKey(
        string? oldIndex,
        string oldForeignKey,
        string fromTable,
        string fromColumn,
        string toTable,
        string toColumn,
        Rule cascade
    )
    {
        Delete.ForeignKey(oldForeignKey).OnTable(fromTable);
        if (oldIndex != null)
            Delete.Index(oldIndex).OnTable(fromTable);
        Create.Index("IX_" + fromTable + "_" + fromColumn).OnTable(fromTable).OnColumn(fromColumn);
        var foreignKeyName = "FK_" + fromTable + "_" + fromColumn + "_" + toTable + "_" + toColumn;
        if (foreignKeyName.Length > 64)
        {
            foreignKeyName = "FK_" + fromTable + "_" + fromColumn;
        }
        Create.ForeignKey(foreignKeyName)
            .FromTable(fromTable).ForeignColumn(fromColumn)
            .ToTable(toTable).PrimaryColumn(toColumn)
            .OnDeleteOrUpdate(cascade);
    }

    public override void Down()
    {
        throw new NotSupportedException();
    }
}