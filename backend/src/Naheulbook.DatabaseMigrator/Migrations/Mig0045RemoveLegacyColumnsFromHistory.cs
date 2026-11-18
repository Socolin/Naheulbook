using System.Data;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

/// <summary>
/// History entries used to reference the items / modifiers / effects / aptitudes they were about, which prevented
/// deleting those (and showed the current name instead of the name at the time of the log).
/// Now everything needed to display an entry is stored as a snapshot in the `data` json column:
/// <list type="bullet">
/// <item><c>item</c> -> <c>{"item": {"name": "...", "quantity": 2, "icon": {...}}}</c></item>
/// <item><c>modifier</c> -> <c>{"modifier": {"name": "..."}}</c></item>
/// <item><c>effect</c> -> <c>{"effect": {"name": "..."}}</c></item>
/// <item><c>aptitudeId</c> -> <c>{"aptitude": {"id": "...", "name": "..."}}</c></item>
/// <item><c>info</c> -> <c>{"level": 3}</c> for LEVEL_UP, <c>{"text": "..."}</c> for EVENT_RP, <c>{"info": "..."}</c> otherwise</item>
/// </list>
/// Then the legacy columns are dropped.
/// </summary>
[Migration(45)]
public class Mig0045RemoveLegacyColumnsFromHistory : Migration
{
    private const string CharacterHistoryTable = "character_history_entries";
    private const string GroupHistoryTable = "group_history_entries";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public override void Up()
    {
        Execute.WithConnection(MigrateCharacterHistoryData);
        Execute.WithConnection(MigrateGroupHistoryData);

        Delete.ForeignKey("FK_character_history_character_modifier_modifier").OnTable(CharacterHistoryTable);
        Delete.ForeignKey("FK_character_history_effect_effect").OnTable(CharacterHistoryTable);
        Delete.ForeignKey("FK_character_history_item_item").OnTable(CharacterHistoryTable);
        Delete.ForeignKey("FK_character_history_entries_aptitudeId_aptitudes_id").OnTable(CharacterHistoryTable);

        Delete.Column("effect")
            .Column("item")
            .Column("modifier")
            .Column("aptitudeId")
            .Column("info")
            .FromTable(CharacterHistoryTable);

        Delete.Column("info").FromTable(GroupHistoryTable);
    }

    public override void Down()
    {
        // Links to items / modifiers / effects / aptitudes cannot be restored, only the columns and the `info` text.
        Alter.Table(CharacterHistoryTable)
            .AddColumn("effect").AsInt64().Nullable()
            .ForeignKey("FK_character_history_effect_effect", "effects", "id")
            .AddColumn("item").AsInt64().Nullable()
            .ForeignKey("FK_character_history_item_item", "items", "id").OnDelete(Rule.SetNull)
            .AddColumn("modifier").AsInt64().Nullable()
            .ForeignKey("FK_character_history_character_modifier_modifier", "character_modifiers", "id")
            .AddColumn("aptitudeId").AsGuid().Nullable()
            .ForeignKey("FK_character_history_entries_aptitudeId_aptitudes_id", "aptitudes", "id")
            .AddColumn("info").AsCustom("longtext").Nullable();

        Alter.Table(GroupHistoryTable)
            .AddColumn("info").AsCustom("longtext").Nullable();

        Execute.Sql($"UPDATE `{CharacterHistoryTable}` SET `info` = JSON_UNQUOTE(JSON_EXTRACT(`data`, '$.level')) WHERE `action` = 'LEVEL_UP'");
        Execute.Sql($"UPDATE `{CharacterHistoryTable}` SET `info` = JSON_UNQUOTE(JSON_EXTRACT(`data`, '$.info')) WHERE `action` <> 'LEVEL_UP'");
        Execute.Sql($"UPDATE `{GroupHistoryTable}` SET `info` = JSON_UNQUOTE(JSON_EXTRACT(`data`, '$.text')) WHERE `action` = 'EVENT_RP'");
        Execute.Sql($"UPDATE `{GroupHistoryTable}` SET `info` = JSON_UNQUOTE(JSON_EXTRACT(`data`, '$.info')) WHERE `action` <> 'EVENT_RP'");
    }

    private static void MigrateCharacterHistoryData(IDbConnection connection, IDbTransaction transaction)
    {
        var updates = new List<(long Id, string Data)>();

        using (var select = connection.CreateCommand())
        {
            select.Transaction = transaction;
            select.CommandText = $"""
                SELECT h.`id`, h.`action`, h.`data`, h.`info`,
                       h.`item`, i.`data`, it.`name`,
                       h.`modifier`, m.`name`,
                       h.`effect`, e.`name`,
                       h.`aptitudeId`, a.`name`
                FROM `{CharacterHistoryTable}` h
                LEFT JOIN `items` i ON i.`id` = h.`item`
                LEFT JOIN `item_templates` it ON it.`id` = i.`itemTemplateId`
                LEFT JOIN `character_modifiers` m ON m.`id` = h.`modifier`
                LEFT JOIN `effects` e ON e.`id` = h.`effect`
                LEFT JOIN `aptitudes` a ON a.`id` = h.`aptitudeId`
                WHERE h.`item` IS NOT NULL
                   OR h.`modifier` IS NOT NULL
                   OR h.`effect` IS NOT NULL
                   OR h.`aptitudeId` IS NOT NULL
                   OR h.`info` IS NOT NULL
                """;
            using var reader = select.ExecuteReader();
            while (reader.Read())
            {
                var id = Convert.ToInt64(reader.GetValue(0));
                var action = reader.GetString(1);
                var data = ParseData(GetNullableString(reader, 2));

                if (!reader.IsDBNull(4))
                    data["item"] = BuildItemSnapshot(GetNullableString(reader, 5), GetNullableString(reader, 6));
                if (!reader.IsDBNull(7))
                    data["modifier"] = new JsonObject {["name"] = GetNullableString(reader, 8)};
                if (!reader.IsDBNull(9))
                    data["effect"] = new JsonObject {["name"] = GetNullableString(reader, 10)};
                if (!reader.IsDBNull(11))
                    data["aptitude"] = new JsonObject {["id"] = reader.GetValue(11).ToString(), ["name"] = GetNullableString(reader, 12)};

                if (GetNullableString(reader, 3) is { } info)
                {
                    if (action == "LEVEL_UP" && int.TryParse(info.Trim(), out var level))
                        data["level"] = level;
                    else
                        data.TryAdd("info", info);
                }

                updates.Add((id, data.ToJsonString(JsonOptions)));
            }
        }

        UpdateData(connection, transaction, CharacterHistoryTable, updates);
    }

    private static void MigrateGroupHistoryData(IDbConnection connection, IDbTransaction transaction)
    {
        var updates = new List<(long Id, string Data)>();

        using (var select = connection.CreateCommand())
        {
            select.Transaction = transaction;
            select.CommandText = $"SELECT `id`, `action`, `data`, `info` FROM `{GroupHistoryTable}` WHERE `info` IS NOT NULL";
            using var reader = select.ExecuteReader();
            while (reader.Read())
            {
                var id = Convert.ToInt64(reader.GetValue(0));
                var action = reader.GetString(1);
                var data = ParseData(GetNullableString(reader, 2));
                var info = reader.GetString(3);

                data.TryAdd(action == "EVENT_RP" ? "text" : "info", info);

                updates.Add((id, data.ToJsonString(JsonOptions)));
            }
        }

        UpdateData(connection, transaction, GroupHistoryTable, updates);
    }

    private static JsonObject BuildItemSnapshot(string? itemDataJson, string? itemTemplateName)
    {
        var snapshot = new JsonObject();
        var itemData = string.IsNullOrEmpty(itemDataJson) ? null : JsonNode.Parse(itemDataJson) as JsonObject;

        var name = itemData?["name"] is JsonValue nameValue && nameValue.TryGetValue<string>(out var n) && !string.IsNullOrEmpty(n)
            ? n
            : itemTemplateName;
        snapshot["name"] = name;

        if (itemData?["quantity"] is JsonValue quantity)
            snapshot["quantity"] = quantity.DeepClone();
        if (itemData?["icon"] is { } icon)
            snapshot["icon"] = icon.DeepClone();

        return snapshot;
    }

    private static JsonObject ParseData(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new JsonObject();
        return JsonNode.Parse(json) as JsonObject ?? new JsonObject();
    }

    private static string? GetNullableString(IDataRecord reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal).ToString();
    }

    private static void UpdateData(IDbConnection connection, IDbTransaction transaction, string table, List<(long Id, string Data)> updates)
    {
        foreach (var (id, data) in updates)
        {
            using var update = connection.CreateCommand();
            update.Transaction = transaction;
            update.CommandText = $"UPDATE `{table}` SET `data` = @data WHERE `id` = @id";
            AddParameter(update, "@data", data);
            AddParameter(update, "@id", id);
            update.ExecuteNonQuery();
        }
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
