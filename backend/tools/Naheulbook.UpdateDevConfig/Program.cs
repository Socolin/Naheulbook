using Naheulbook.Tools.Shared;
using Newtonsoft.Json.Linq;


var configResolver = new ConfigResolver();
var mysqlPort = await configResolver.GetMysqlPortAsync();
var redisPort = await configResolver.GetRedisPortAsync();
var naheulbookRootFolder = FileSystemHelper.FindDirectoryContaining("Naheulbook.sln");
var mysqlConnectionString = $"Server=localhost;Database=naheulbook;Uid=naheulbook;Pwd=naheulbook;SslMode=None;CharSet=utf8;AllowPublicKeyRetrieval=True;Port={mysqlPort}";
var redisConnectionString = $"localhost:{redisPort},abortConnect=false";

Console.WriteLine("Mysql port:" + mysqlPort);
Console.WriteLine("Redis port:" + redisPort);

UpdateConfigFile(Path.Combine(naheulbookRootFolder, "src", "Naheulbook.Web", "appsettings.local.json"),
    new Dictionary<string, string>
    {
        ["DefaultConnection"] = mysqlConnectionString,
        ["Redis"] = redisConnectionString,
    });

UpdateConfigFile(Path.Combine(naheulbookRootFolder, "tools", "Naheulbook.DatabaseMigrator.Cli", "appsettings.local.json"),
    new Dictionary<string, string>
    {
        ["DefaultConnection"] = mysqlConnectionString,
    });


void UpdateConfigFile(string file, Dictionary<string, string> values)
{
    var configJObject = !File.Exists(file) ? new JObject() : JObject.Parse(File.ReadAllText(file));
    if (!configJObject.ContainsKey("ConnectionStrings"))
        configJObject.Add("ConnectionStrings", new JObject());
    var connectionStringJObject = configJObject.Value<JObject>("ConnectionStrings");

    foreach (var (key, value) in values)
        connectionStringJObject![key] = value;

    if (File.Exists(file))
    {
        var backup = Path.GetTempFileName();
        File.Copy(file, backup, true);
        Console.WriteLine($"Update file: {file} (backup: {backup})");
    }
    else
    {
        Console.WriteLine($"Create config file: {file}");
    }

    File.WriteAllText(file, configJObject!.ToString());
}