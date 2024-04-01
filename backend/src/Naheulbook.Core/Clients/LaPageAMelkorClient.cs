using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Naheulbook.Shared.Extensions;
using Newtonsoft.Json;

namespace Naheulbook.Core.Clients;

public interface ILaPageAMelkorClient
{
    Task<ICollection<string>> GetRandomNameAsync(string url);
}

public class LaPageAMelkorClient(HttpClient client) : ILaPageAMelkorClient
{
    public async Task<ICollection<string>> GetRandomNameAsync(string url)
    {
        var response = await client.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new GenericHttpClientException(url, (int) response.StatusCode, responseContent);
        }

        var randomNames = JsonConvert.DeserializeObject<List<string>>(responseContent);
        return randomNames.NotNull();
    }
}