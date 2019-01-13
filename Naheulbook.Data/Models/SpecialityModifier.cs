namespace Naheulbook.Data.Models
{
    public class SpecialityModifier
    {
        public int Id { get; set; }
        public string Stat { get; set; }
        public int Value { get; set; }

        public int SpecialityId { get; set; }
        public virtual Speciality Speciality { get; set; }
    }
}