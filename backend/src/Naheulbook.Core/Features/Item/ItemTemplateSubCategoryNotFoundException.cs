namespace Naheulbook.Core.Features.Item;

[Serializable]
public class ItemTemplateSubCategoryNotFoundException : Exception
{
    public int ItemTemplateSubCategoryId { get; }
    public string? ItemTemplateSubCategoryTechName { get; }

    public ItemTemplateSubCategoryNotFoundException(int itemTemplateSubCategoryId)
    {
        ItemTemplateSubCategoryId = itemTemplateSubCategoryId;
    }

    public ItemTemplateSubCategoryNotFoundException(string itemTemplateSubCategoryTechName)
    {
        ItemTemplateSubCategoryTechName = itemTemplateSubCategoryTechName;
    }
}