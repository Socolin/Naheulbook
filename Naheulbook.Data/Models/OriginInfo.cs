namespace Naheulbook.Data.Models
{
    public class OriginInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}