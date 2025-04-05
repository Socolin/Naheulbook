namespace Naheulbook.Core.Features.Item;

[Serializable]
public class ItemTemplateNotFoundException(Guid itemTemplateId) : Exception
{
    public Guid ItemTemplateId { get; } = itemTemplateId;
}