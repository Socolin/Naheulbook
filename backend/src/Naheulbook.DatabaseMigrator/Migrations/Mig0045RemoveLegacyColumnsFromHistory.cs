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
/// <item><c>item</c> -> <c>{"item": {"templateId": "...", "data": {...}, "modifiers": [...]}}</c>: a copy of the item, as returned by the API</item>
/// <item><c>modifier</c> -> <c>{"modifier": {"name": "...", "values": [{"stat": "FO", "type": "ADD", "value": 1}]}}</c></item>
/// <item><c>effect</c> -> <c>{"effect": {"name": "..."}}</c></item>
/// <item><c>aptitudeId</c> -> <c>{"aptitude": {"id": "...", "name": "..."}}</c></item>
/// <item><c>info</c> -> <c>{"level": 3}</c> for LEVEL_UP, <c>{"text": "..."}</c> for EVENT_RP, <c>{"info": "..."}</c> otherwise</item>
/// <item>GIVE_ITEM / GIVEN_ITEM -> <c>{"characterName": "..."}</c>: the other character of the exchange, found by pairing
/// the entries of the giver and of the receiver (see <see cref="FillGiveItemCharacterNames"/>)</item>
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
        var modifierValues = LoadHistoryModifierValues(connection, transaction);
        var updates = new List<(long Id, JsonObject Data)>();
        var giveItemEntries = new List<GiveItemEntry>();

        using (var select = connection.CreateCommand())
        {
            select.Transaction = transaction;
            select.CommandText = $"""
                SELECT h.`id`, h.`action`, h.`data`, h.`info`,
                       h.`item`, i.`data`, it.`name`,
                       h.`modifier`, m.`name`,
                       h.`effect`, e.`name`,
                       h.`aptitudeId`, a.`name`,
                       h.`character`, c.`name`, h.`date`,
                       i.`itemTemplateId`, i.`modifiers`
                FROM `{CharacterHistoryTable}` h
                JOIN `characters` c ON c.`id` = h.`character`
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
                   OR h.`action` IN ('GIVE_ITEM', 'GIVEN_ITEM')
                """;
            using var reader = select.ExecuteReader();
            while (reader.Read())
            {
                var id = Convert.ToInt64(reader.GetValue(0));
                var action = reader.GetString(1);
                var data = ParseData(GetNullableString(reader, 2));

                if (!reader.IsDBNull(4))
                    data["item"] = BuildItemSnapshot(GetNullableString(reader, 16), GetNullableString(reader, 5), GetNullableString(reader, 17), GetNullableString(reader, 6));
                if (!reader.IsDBNull(7))
                {
                    var modifierId = Convert.ToInt64(reader.GetValue(7));
                    data["modifier"] = new JsonObject
                    {
                        ["name"] = GetNullableString(reader, 8),
                        ["values"] = modifierValues.TryGetValue(modifierId, out var values)
                            ? new JsonArray(values.Select(v => (JsonNode)v.DeepClone()).ToArray())
                            : new JsonArray(),
                    };
                }
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

                if (action is "GIVE_ITEM" or "GIVEN_ITEM")
                {
                    giveItemEntries.Add(new GiveItemEntry(
                        action == "GIVE_ITEM",
                        Convert.ToInt64(reader.GetValue(13)),
                        GetNullableString(reader, 14),
                        Convert.ToDateTime(reader.GetValue(15)),
                        reader.IsDBNull(4) ? null : Convert.ToInt64(reader.GetValue(4)),
                        data["item"]?["data"]?["name"]?.GetValue<string>(),
                        data
                    ));
                }

                updates.Add((id, data));
            }
        }

        FillGiveItemCharacterNames(giveItemEntries);

        UpdateData(connection, transaction, CharacterHistoryTable, updates.Select(u => (u.Id, u.Data.ToJsonString(JsonOptions))).ToList());
    }

    private record GiveItemEntry(bool IsGiver, long CharacterId, string? CharacterName, DateTime Date, long? ItemId, string? ItemName, JsonObject Data);

    /// <summary>
    /// When an item was given, the giver got a GIVE_ITEM entry and the receiver a GIVEN_ITEM entry, at the same time but
    /// without any link between them. Pair them to store the name of the other character in each entry.
    /// The item is the same when the whole stack was given, but when only a part was given the receiver got a new item
    /// (with the same name), so try in order: same item, same item name, then the only receiver at that time.
    /// Entries that cannot be paired without ambiguity are left without a character name.
    /// </summary>
    private static void FillGiveItemCharacterNames(List<GiveItemEntry> entries)
    {
        var receivers = entries.Where(e => !e.IsGiver).ToList();
        var pairedReceivers = new HashSet<GiveItemEntry>();

        foreach (var giver in entries.Where(e => e.IsGiver))
        {
            var candidates = receivers
                .Where(r => !pairedReceivers.Contains(r))
                .Where(r => r.CharacterId != giver.CharacterId)
                .Where(r => Math.Abs((r.Date - giver.Date).TotalSeconds) <= 2)
                .ToList();

            var receiver = SingleOrNull(candidates.Where(r => giver.ItemId != null && r.ItemId == giver.ItemId))
                           ?? SingleOrNull(candidates.Where(r => giver.ItemName != null && r.ItemName == giver.ItemName))
                           ?? (candidates.Select(r => r.CharacterId).Distinct().Count() == 1 ? candidates.MinBy(r => Math.Abs((r.Date - giver.Date).Ticks)) : null);
            if (receiver == null)
                continue;

            pairedReceivers.Add(receiver);
            giver.Data["characterName"] = receiver.CharacterName;
            receiver.Data["characterName"] = giver.CharacterName;
        }
    }

    private static GiveItemEntry? SingleOrNull(IEnumerable<GiveItemEntry> entries)
    {
        var list = entries.Take(2).ToList();
        return list.Count == 1 ? list[0] : null;
    }

    private static Dictionary<long, List<JsonObject>> LoadHistoryModifierValues(IDbConnection connection, IDbTransaction transaction)
    {
        var valuesByModifierId = new Dictionary<long, List<JsonObject>>();

        using var select = connection.CreateCommand();
        select.Transaction = transaction;
        select.CommandText = $"""
            SELECT v.`characterModifierId`, v.`stat`, v.`type`, v.`value`
            FROM `character_modifier_values` v
            WHERE v.`characterModifierId` IN (SELECT h.`modifier` FROM `{CharacterHistoryTable}` h WHERE h.`modifier` IS NOT NULL)
            ORDER BY v.`id`
            """;
        using var reader = select.ExecuteReader();
        while (reader.Read())
        {
            var modifierId = Convert.ToInt64(reader.GetValue(0));
            if (!valuesByModifierId.TryGetValue(modifierId, out var values))
                valuesByModifierId[modifierId] = values = [];
            values.Add(new JsonObject
            {
                ["stat"] = reader.GetValue(1).ToString(),
                ["type"] = reader.GetValue(2).ToString(),
                ["value"] = Convert.ToInt32(reader.GetValue(3)),
            });
        }

        return valuesByModifierId;
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

    private static JsonObject BuildItemSnapshot(string? itemTemplateId, string? itemDataJson, string? itemModifiersJson, string? itemTemplateName)
    {
        var itemData = string.IsNullOrEmpty(itemDataJson) ? new JsonObject() : JsonNode.Parse(itemDataJson) as JsonObject ?? new JsonObject();
        if (itemData["name"] is not JsonValue nameValue || !nameValue.TryGetValue<string>(out var name) || string.IsNullOrEmpty(name))
            itemData["name"] = itemTemplateName;

        var snapshot = new JsonObject
        {
            ["templateId"] = itemTemplateId,
            ["data"] = itemData,
        };
        if (!string.IsNullOrEmpty(itemModifiersJson) && JsonNode.Parse(itemModifiersJson) is JsonArray modifiers)
            snapshot["modifiers"] = modifiers;

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
