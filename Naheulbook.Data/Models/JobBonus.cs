namespace Naheulbook.Data.Models
{
    public class JobBonus
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Flags { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; }
    }
}