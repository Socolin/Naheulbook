using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class MerchantResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}