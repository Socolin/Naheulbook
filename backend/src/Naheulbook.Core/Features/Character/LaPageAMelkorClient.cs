using System.ComponentModel.DataAnnotations;
using Naheulbook.Shared.Extensions;
using Newtonsoft.Json;

namespace Naheulbook.Core.Features.Character;

public interface ILaPageAMelkorClient
{
    Task<ICollection<string>> GetRandomNameAsync(string url, CancellationToken cancellationToken = default);
}

public class LaPageAMelkorClient(HttpClient client) : ILaPageAMelkorClient
{
    [Serializable]
    public class Options
    {
        [Required]
        public required string Url { get; set; }
    }

    public async Task<ICollection<string>> GetRandomNameAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(url, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new GenericHttpClientException(url, (int) response.StatusCode, responseContent);
        }

        var randomNames = JsonConvert.DeserializeObject<List<string>>(responseContent);
        return randomNames.NotNull();
    }
}