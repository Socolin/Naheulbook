namespace Naheulbook.Tests.Functional.Code.Constants
{
    public static class DefaultTestConfigurations
    {
        public const string NaheulbookDbName = "naheulbook_test";
        public static string NaheulbookTestConnectionString => $"Server=localhost;Database={NaheulbookDbName};Uid=naheulbook;Pwd=naheulbook;SslMode=None;CharSet=utf8;";
    }
}