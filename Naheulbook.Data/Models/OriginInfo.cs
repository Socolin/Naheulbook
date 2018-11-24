namespace Naheulbook.Data.Models
{
    public class OriginInfo
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public long OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}