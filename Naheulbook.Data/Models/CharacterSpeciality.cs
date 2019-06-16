namespace Naheulbook.Data.Models
{
    public class CharacterSpeciality
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; }

        public int SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
    }
}