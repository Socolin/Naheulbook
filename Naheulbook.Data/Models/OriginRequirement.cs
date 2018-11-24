namespace Naheulbook.Data.Models
{
    public class OriginRequirement
    {
        public long Id { get; set; }

        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }

        public long OriginId { get; set; }
        public Origin Origin { get; set; }

        public string StatName { get; set; }
        public Stat Stat { get; set; }
    }
}