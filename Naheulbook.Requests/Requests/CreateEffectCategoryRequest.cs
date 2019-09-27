namespace Naheulbook.Requests.Requests
{
    public class CreateEffectCategoryRequest
    {
        public string Name { get; set; } = null!;
        public short DiceCount { get; set; }
        public short DiceSize { get; set; }
        public string? Note { get; set; }
        public int TypeId { get; set; }
    }
}