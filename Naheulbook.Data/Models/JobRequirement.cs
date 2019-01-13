namespace Naheulbook.Data.Models
{
    public class JobRequirement
    {
        public int Id { get; set; }

        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }

        public string StatName { get; set; }
        public Stat Stat { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; }
    }
}