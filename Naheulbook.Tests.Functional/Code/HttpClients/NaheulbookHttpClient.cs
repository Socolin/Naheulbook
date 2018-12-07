using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public Task<HttpResponseMessage> PostAsync(string uri, object requestContent)
        {
            if (requestContent is HttpContent httpContent)
                return base.PostAsync(uri, httpContent);
            return base.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));
        }

        public async Task<T> PostAndParseJsonResultAsync<T>(string uri, object requestContent) where T : class
        {
            var response = await base.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var contentJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentJson);
        }
    }
}