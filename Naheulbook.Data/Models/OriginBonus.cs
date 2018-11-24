namespace Naheulbook.Data.Models
{
    public class OriginBonus
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string Flags { get; set; }

        public long OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}