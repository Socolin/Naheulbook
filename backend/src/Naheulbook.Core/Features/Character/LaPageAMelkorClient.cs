using System.ComponentModel.DataAnnotations;
using Naheulbook.Shared.Extensions;
using Newtonsoft.Json;

namespace Naheulbook.Core.Features.Character;

public interface ILaPageAMelkorClient
{
    Task<ICollection<string>> GetRandomNameAsync(string url);
}

public class LaPageAMelkorClient(HttpClient client) : ILaPageAMelkorClient
{
    [Serializable]
    public class Options
    {
        [Required]
        public required string Url { get; set; }
    }

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