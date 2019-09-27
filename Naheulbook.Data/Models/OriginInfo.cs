namespace Naheulbook.Data.Models
{
    public class OriginInfo
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public int OriginId { get; set; }
        public Origin Origin { get; set; } = null!;
    }
}