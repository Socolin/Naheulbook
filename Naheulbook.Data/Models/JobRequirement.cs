namespace Naheulbook.Data.Models
{
    public class JobRequirement
    {
        public long Id { get; set; }

        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }

        public string StatName { get; set; }
        public Stat Stat { get; set; }

        public long JobId { get; set; }
        public Job Job { get; set; }
    }
}