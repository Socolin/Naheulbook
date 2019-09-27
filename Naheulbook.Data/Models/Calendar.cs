namespace Naheulbook.Data.Models
{
    public class Calendar
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int StartDay { get; set; }
        public int EndDay { get; set; }
        public string? Note { get; set; }
    }
}