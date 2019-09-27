namespace Naheulbook.Data.Models
{
    public class JobRestrict
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string? Flags { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
    }
}