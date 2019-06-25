namespace Naheulbook.Requests.Requests
{
    public class PatchCharacterStatsRequest
    {
        public int? Ev { get; set; }
        public int? Ea { get; set; }
        public short? FatePoint { get; set; }
        public int? Experience { get; set; }
        public string Sex { get; set; }
        public string Name { get; set; }
    }
}