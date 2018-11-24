namespace Naheulbook.Data.Models
{
    public class JobRestrict
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string Flags { get; set; }

        public long JobId { get; set; }
        public Job Job { get; set; }
    }
}