namespace Naheulbook.Data.Models
{
    public class OriginRandomNameUrl
    {
        public int Id { get; set; }

        public string Sex { get; set; }
        public string Url { get; set; }

        public int OriginId { get; set; }
        public Origin Origin { get; set; }
    }
}