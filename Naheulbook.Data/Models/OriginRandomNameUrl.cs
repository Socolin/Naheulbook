using System;

namespace Naheulbook.Data.Models
{
    public class OriginRandomNameUrl
    {
        public int Id { get; set; }

        public string Sex { get; set; } = null!;
        public string Url { get; set; } = null!;

        public Guid OriginId { get; set; }
        public Origin Origin { get; set; } = null!;
    }
}