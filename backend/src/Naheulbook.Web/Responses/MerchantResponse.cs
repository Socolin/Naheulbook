using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class MerchantResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}