namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MerchantEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int GroupId { get; set; }
}