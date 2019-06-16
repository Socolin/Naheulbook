namespace Naheulbook.Data.Models
{
    public class CharacterModifierValue
    {
        public int Id { get; set; }
        public int CharacterModifierId { get; set; }
        public string StatName { get; set; }
        public int Type { get; set; }
        public short Value { get; set; }
    }
}