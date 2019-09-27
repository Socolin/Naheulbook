namespace Naheulbook.Data.Models
{
    public class SpecialitySpecial
    {
        public int Id { get; set; }
        public bool IsBonus { get; set; }
        public string Description { get; set; } = null!;
        public string? Flags { get; set; }

        public int SpecialityId { get; set; }
        public Speciality Speciality { get; set; } = null!;
    }
}