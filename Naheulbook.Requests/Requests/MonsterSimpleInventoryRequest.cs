namespace Naheulbook.Requests.Requests
{
    public class MonsterSimpleInventoryRequest
    {
        public int? Id { get; set; }
        public int ItemTemplateId { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public int MinUg { get; set; }
        public int MaxUg { get; set; }
        public float Chance { get; set; }
        public bool Hidden { get; set; }
    }
}