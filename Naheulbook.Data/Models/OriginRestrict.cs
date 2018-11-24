namespace Naheulbook.Data.Models
{
    public class OriginRestrict
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Flags { get; set; }

        public long OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}

