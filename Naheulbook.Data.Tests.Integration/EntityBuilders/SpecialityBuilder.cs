using Naheulbook.Data.Models;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class SpecialityBuilder : BuilderBase<Speciality>
    {
        public SpecialityBuilder WithDefaultTestInfo()
        {
            Entity.Name = "some-name";
            Entity.Flags = "[]";
            Entity.Description = "some-description";
            return this;
        }

        public SpecialityBuilder WithJob(Job job)
        {
            Entity.Job = job;
            return this;
        }

        public SpecialityBuilder WithModifier(string statName = "some-speciality-stat", int value = 42)
        {
            Entity.Modifiers.Add(new SpecialityModifier
            {
                Stat = statName,
                Value = value
            });
            return this;
        }

        public SpecialityBuilder WithSpecial(string description = "some-description", bool isBonus = true, string flags = "[]")
        {
            Entity.Specials.Add(new SpecialitySpecial()
            {
                Description = description,
                IsBonus = isBonus,
                Flags = flags
            });
            return this;
        }

    }
}