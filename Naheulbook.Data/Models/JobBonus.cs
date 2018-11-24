namespace Naheulbook.Data.Models
{
    public class JobBonus
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public string Flags { get; set; }

        public long JobId { get; set; }
        public Job Job { get; set; }
    }
}