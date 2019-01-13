namespace Naheulbook.Data.Models
{
    public class Slot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TechName { get; set; }
        public short Count { get; set; }
        public bool? Stackable { get; set; }
    }
}