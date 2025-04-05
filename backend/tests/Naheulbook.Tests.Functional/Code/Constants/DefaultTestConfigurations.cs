using Naheulbook.TestUtils;

namespace Naheulbook.Tests.Functional.Code.Constants;

public static class DefaultTestConfigurations
{
    public const string NaheulbookDbName = "naheulbook_test";
    public const string NaheulbookDbUserName = "naheulbook_test";

    public static string NaheulbookTestConnectionString => $"Server={Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost"};" +
                                                           $"Database={NaheulbookDbName};" +
                                                           $"Uid={NaheulbookDbUserName};" +
                                                           $"Port={DockerServicePortFinder.GetPortForServiceInDocker("scripts-naheulbook_dev_env_mysql-1", "3306/tcp")?.ToString() ?? Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306"};" +
                                                           "Pwd=naheulbook;" +
                                                           "SslMode=None;" +
                                                           "CharSet=utf8;" +
                                                           "AllowPublicKeyRetrieval=True;" +
                                                           "Allow User Variables=True";
}