namespace Naheulbook.Data.Models
{
    public class JobOriginBlacklist
    {
        public long Id { get; set; }

        public long JobId { get; set; }
        public Job Job { get; set; }

        public long OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}