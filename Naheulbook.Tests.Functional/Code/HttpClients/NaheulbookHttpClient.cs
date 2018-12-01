using System;
using System.Net.Http;

namespace Naheulbook.Tests.Functional.Code.HttpClients
{
    public class NaheulbookHttpClient : HttpClient
    {
        public NaheulbookHttpClient(string uri)
        {
            BaseAddress = new Uri(uri);
            DefaultRequestHeaders.UserAgent.ParseAdd("NaheulbookTest");
            DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");
            DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }
    }
}