namespace Naheulbook.Data.Models
{
    public class OriginRequirement
    {
        public int Id { get; set; }

        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; } = null!;

        public string StatName { get; set; } = null!;
        public Stat Stat { get; set; } = null!;
    }
}