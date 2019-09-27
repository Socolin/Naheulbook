namespace Naheulbook.Data.Models
{
    public class ItemTemplateModifier
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string? Special { get; set; }
        public string Type { get; set; } = null!;

        public int ItemTemplateId { get; set; }
        public ItemTemplate ItemTemplate { get; set; } = null!;

        public int? RequireJobId { get; set; }
        public Job? RequireJob { get; set; }

        public int? RequireOriginId { get; set; }
        public Origin? RequireOrigin { get; set; }

        public string StatName { get; set; } = null!;
        public Stat Stat { get; set; } = null!;
    }
}