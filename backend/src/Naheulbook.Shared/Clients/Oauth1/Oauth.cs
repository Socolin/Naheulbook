using System.Security.Cryptography;
using System.Text;
using Naheulbook.Shared.Utils;


namespace Naheulbook.Shared.Clients.Oauth1;

public class Oauth(string consumerKey, string consumerSecret, string requestUrl)
{
    public string Method { get; set; } = "POST";
    public string RequestUrl { get; } = requestUrl;
    public string? AccessSecret { get; set; }
    public string SignatureMethod { get; set; } = "HMAC-SHA1";
    public string Version { get; set; } = "1.0";
    private IDictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
    private IDictionary<string, string> OauthParameters { get; } = new Dictionary<string, string>();

    private void UpdateDefaultOauthParameters()
    {
        OauthParameters["nonce"] = RngHelper.GetRandomHexString(10);
        OauthParameters["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        OauthParameters["consumer_key"] = consumerKey;
        OauthParameters["signature_method"] = SignatureMethod;
        OauthParameters["version"] = Version;
    }

    public void AddOauthParameter(string key, string value)
    {
        OauthParameters[key] = value;
    }

    public void AddParameter(string key, string value)
    {
        Parameters[key] = value;
    }

    private string ParametersAsString()
    {
        IDictionary<string, string> allParams = new Dictionary<string, string>();
        foreach (var p in OauthParameters)
        {
            allParams[$"oauth_{p.Key}"] = p.Value;
        }

        foreach (var p in Parameters)
        {
            allParams[p.Key] = p.Value;
        }

        var paramsAsString = string.Join("&", allParams
            .OrderBy(k => k.Key)
            .Select(s => Uri.EscapeDataString(s.Key) + "=" + Uri.EscapeDataString(s.Value)));

        return paramsAsString;
    }


    private string SignatureBaseString()
    {
        var paramString = ParametersAsString();
        var signatureBaseString = $"{Method}&{Uri.EscapeDataString(RequestUrl)}&{Uri.EscapeDataString(paramString)}";

        return signatureBaseString;
    }

    private string SignatureKey()
    {
        var signatureKey = Uri.EscapeDataString(consumerSecret) + "&";
        if (!string.IsNullOrEmpty(AccessSecret))
        {
            signatureKey += Uri.EscapeDataString(AccessSecret);
        }

        return signatureKey;
    }

    private string GenerateSignature()
    {
        using var algorithm = new HMACSHA1();
        var signatureKey = SignatureKey();
        algorithm.Key = Encoding.ASCII.GetBytes(signatureKey);
        var signatureBaseString = SignatureBaseString();
        var hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
        return Convert.ToBase64String(hash);
    }

    public string AuthorizationHeader()
    {
        UpdateDefaultOauthParameters();

        OauthParameters["signature"] = GenerateSignature();

        var oauthParams = string.Join(", ", OauthParameters
            .OrderBy(k => k.Key)
            .Select(s => Uri.EscapeDataString($"oauth_{s.Key}") + "=\"" + Uri.EscapeDataString(s.Value) + "\""));

        OauthParameters.Remove("signature");

        return $"OAuth {oauthParams}";
    }

    public async Task<IDictionary<string, string>> DoRequest()
    {
        using var httpClient = new HttpClient();
        var authorizationHeader = AuthorizationHeader();
        httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

        // `!` is added because this issue https://github.com/dotnet/runtime/issues/54367
        using var response = await httpClient.PostAsync(RequestUrl, new FormUrlEncodedContent(Parameters));
        using var content = response.Content;
        var result = await content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return result.Split('&')
                .Select(s => s.Split(['='], 2))
                .ToDictionary(s => s[0], s => s[1]);
        }

        throw new OAuthException(RequestUrl, (int) response.StatusCode, result);
    }
}