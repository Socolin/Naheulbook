using System.Net.Http;
using Naheulbook.Tests.Functional.Code.Servers;

namespace Naheulbook.Tests.Functional.Code.HttpClients
{
    public class NaheulbookHttpClient : HttpClient
    {
        public NaheulbookHttpClient()
        {
            BaseAddress = NaheulbookApiServer.Url;
            DefaultRequestHeaders.UserAgent.ParseAdd("NaheulbookTest");
            DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");
            DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }
    }
}