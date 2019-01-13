namespace Naheulbook.Data.Models
{
    public class OriginRequirement
    {
        public int Id { get; set; }

        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; }

        public string StatName { get; set; }
        public Stat Stat { get; set; }
    }
}