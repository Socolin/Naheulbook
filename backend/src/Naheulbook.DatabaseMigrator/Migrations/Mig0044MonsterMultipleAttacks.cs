using System.Data;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using FluentMigrator;

namespace Naheulbook.DatabaseMigrator.Migrations;

/// <summary>
/// Convert the `at` / `dmg` fields of monsters and monster templates data into an `attacks` list:
/// <c>{"at": 8, "dmg": "1D+2"}</c> -> <c>{"attacks": [{"at": 8, "dmg": "1D+2"}]}</c>
///
/// Only the simple cases are migrated here: `at` is a number (or missing when there is no `dmg` either)
/// and `dmg` is missing / empty or a single damage expression like `1D+4`.
/// Rows that do not match are left untouched and will be handled by a dedicated custom migration.
/// </summary>
[Migration(44)]
public partial class Mig0044MonsterMultipleAttacks : Migration
{
    private static readonly string[] Tables = ["monster_templates", "monsters"];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    [GeneratedRegex(@"^\s*(\d*\s*[dD]\s*\d*\s*([+-]\s*\d+\s*)?|\d+\s*)$")]
    private static partial Regex SimpleDamageRegex();

    public override void Up()
    {
        foreach (var table in Tables)
            Execute.WithConnection((connection, transaction) => UpdateAllRows(connection, transaction, table, ConvertToAttacks));
    }

    public override void Down()
    {
        foreach (var table in Tables)
            Execute.WithConnection((connection, transaction) => UpdateAllRows(connection, transaction, table, ConvertFromAttacks));
    }

    private static void UpdateAllRows(IDbConnection connection, IDbTransaction transaction, string table, Func<JsonObject, bool> convert)
    {
        var updates = new List<(long Id, string Data)>();

        using (var select = connection.CreateCommand())
        {
            select.Transaction = transaction;
            select.CommandText = $"SELECT `id`, `data` FROM `{table}` WHERE `data` IS NOT NULL";
            using var reader = select.ExecuteReader();
            while (reader.Read())
            {
                var id = Convert.ToInt64(reader.GetValue(0));
                if (JsonNode.Parse(reader.GetString(1)) is not JsonObject data)
                    continue;
                if (convert(data))
                    updates.Add((id, data.ToJsonString(JsonOptions)));
            }
        }

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

    private static bool ConvertToAttacks(JsonObject data)
    {
        if (data.ContainsKey("attacks"))
            return false;

        var hasAt = TryGetNode(data, "at", out var atNode);
        var hasDmg = TryGetNode(data, "dmg", out var dmgNode);

        if (!hasAt && !hasDmg)
        {
            data.Remove("at");
            data.Remove("dmg");
            data["attacks"] = new JsonArray();
            return true;
        }

        if (!hasAt || !TryGetInt(atNode!, out var at))
            return false;

        string? dmg = null;
        if (hasDmg)
        {
            if (dmgNode is not JsonValue dmgValue || !dmgValue.TryGetValue(out dmg))
                return false;
            if (!SimpleDamageRegex().IsMatch(dmg))
                return false;
            dmg = dmg.Trim();
        }

        var attack = new JsonObject {["at"] = at};
        if (!string.IsNullOrEmpty(dmg))
            attack["dmg"] = dmg;

        data.Remove("at");
        data.Remove("dmg");
        data["attacks"] = new JsonArray(attack);
        return true;
    }

    private static bool ConvertFromAttacks(JsonObject data)
    {
        if (!data.TryGetPropertyValue("attacks", out var attacksNode))
            return false;

        data.Remove("attacks");
        if (attacksNode is JsonArray {Count: > 0} attacks && attacks[0] is JsonObject firstAttack)
        {
            data["at"] = firstAttack["at"]?.DeepClone();
            data["dmg"] = firstAttack["dmg"]?.DeepClone();
        }

        return true;
    }

    /// <summary>
    /// Returns true when the property exists and is not null (or an empty string)
    /// </summary>
    private static bool TryGetNode(JsonObject data, string propertyName, out JsonNode? node)
    {
        if (!data.TryGetPropertyValue(propertyName, out node) || node is null)
            return false;
        if (node is JsonValue value && value.TryGetValue<string>(out var str) && string.IsNullOrWhiteSpace(str))
            return false;
        return true;
    }

    private static bool TryGetInt(JsonNode node, out int value)
    {
        value = 0;
        if (node is not JsonValue jsonValue)
            return false;
        if (jsonValue.TryGetValue(out value))
            return true;
        if (jsonValue.TryGetValue<double>(out var d) && Math.Abs(d % 1) < double.Epsilon)
        {
            value = (int)d;
            return true;
        }
        if (jsonValue.TryGetValue<string>(out var str) && int.TryParse(str.Trim(), out value))
            return true;
        return false;
    }
}
