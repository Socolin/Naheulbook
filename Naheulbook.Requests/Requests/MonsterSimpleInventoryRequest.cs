namespace Naheulbook.Requests.Requests
{
    public class MonsterSimpleInventoryRequest
    {
        public int Id { get; set; }
        public IdRequest ItemTemplate { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public float Chance { get; set; }
    }
}