namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class EditEffectRequest : CreateEffectRequest
{
    public int SubCategoryId { get; set; }
}