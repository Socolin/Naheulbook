namespace Naheulbook.Data.Models
{
    public class SpecialitySpecial
    {
        public long Id { get; set; }
        public bool IsBonus { get; set; }
        public string Description { get; set; }
        public string Flags { get; set; }

        public long SpecialityId { get; set; }
        public Speciality Speciality { get; set; }
    }
}