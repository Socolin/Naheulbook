using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Naheulbook.Core.Clients
{
    public interface ILaPageAMelkorClient
    {
        Task<ICollection<string>> GetRandomNameAsync(string url);
    }

    public class LaPageAMelkorClient : ILaPageAMelkorClient
    {
        private readonly HttpClient _client;

        public LaPageAMelkorClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<ICollection<string>> GetRandomNameAsync(string url)
        {
            var response = await _client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new GenericHttpClientException(url, (int) response.StatusCode, responseContent);
            }

            return JsonConvert.DeserializeObject<List<string>>(responseContent);
        }
    }
}