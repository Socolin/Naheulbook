namespace Naheulbook.Data.Models
{
    public class JobOriginBlacklist
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}