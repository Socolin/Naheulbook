namespace Naheulbook.Tests.Functional.Code.Constants
{
    public static class DefaultTestConfigurations
    {
        public const string NaheulbookDbName = "naheulbook_test";
        public const string NaheulbookDbUserName = "naheulbook_test";

        public static string NaheulbookTestConnectionString => "Server=localhost;" +
                                                               $"Database={NaheulbookDbName};" +
                                                               $"Uid={NaheulbookDbUserName};" +
                                                               "Pwd=naheulbook;" +
                                                               "SslMode=None;" +
                                                               "CharSet=utf8;" +
                                                               "Allow User Variables=True";
    }
}