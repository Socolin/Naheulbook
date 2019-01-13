namespace Naheulbook.Data.Models
{
    public class OriginRestrict
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Flags { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}

