namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class MonsterTemplateInventoryElementRequest
{
    public int? Id { get; set; }
    public Guid ItemTemplateId { get; set; }
    public int MinCount { get; set; }
    public int MaxCount { get; set; }
    public int MinUg { get; set; }
    public int MaxUg { get; set; }
    public float Chance { get; set; }
    public bool Hidden { get; set; }
}