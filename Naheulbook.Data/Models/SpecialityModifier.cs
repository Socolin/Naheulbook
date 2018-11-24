namespace Naheulbook.Data.Models
{
    public class SpecialityModifier
    {
        public long Id { get; set; }
        public string Stat { get; set; }
        public long Value { get; set; }

        public long SpecialityId { get; set; }
        public virtual Speciality Speciality { get; set; }
    }
}