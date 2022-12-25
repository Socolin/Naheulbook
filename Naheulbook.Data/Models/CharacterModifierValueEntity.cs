namespace Naheulbook.Data.Models
{
    public class CharacterModifierValueEntity
    {
        public int Id { get; set; }
        public int CharacterModifierId { get; set; }
        public string StatName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public short Value { get; set; }
    }
}