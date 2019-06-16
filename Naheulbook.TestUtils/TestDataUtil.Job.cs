using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddJob(Action<Job> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateJob(), customizer);
        }

        public TestDataUtil AddJobWithAllData(Action<Job> customizer = null)
        {
            var suffix = RngUtil.GetRandomHexString(8);

            var job = _defaultEntityCreator.CreateJob(suffix);

            var stat = AddStat().GetLast<Stat>();

            var skill1 = AddSkill().GetLast<Skill>();
            var skill2 = AddSkill().GetLast<Skill>();

            var origin1 = AddOrigin().GetLast<Origin>();
            var origin2 = AddOrigin().GetLast<Origin>();

            job.Bonuses = new List<JobBonus>
            {
                new JobBonus
                {
                    Description = $"some-job-bonus-description-{suffix}"
                }
            };
            job.Requirements = new List<JobRequirement>
            {
                new JobRequirement
                {
                    Stat = stat,
                    MinValue = 2,
                    MaxValue = 4,
                }
            };
            job.Restrictions = new List<JobRestrict>
            {
                new JobRestrict
                {
                    Text = $"some-job-restriction-{suffix}",
                    Flags = "[]"
                }
            };
            job.Skills = new List<JobSkill>
            {
                new JobSkill
                {
                    Default = true,
                    Skill = skill1
                },
                new JobSkill
                {
                    Default = false,
                    Skill = skill2
                },
            };
            job.OriginBlacklist = new List<JobOriginBlacklist>
            {
                new JobOriginBlacklist
                {
                    Origin = origin1
                }
            };
            job.OriginWhitelist = new List<JobOriginWhitelist>
            {
                new JobOriginWhitelist
                {
                    Origin = origin2
                }
            };

            SaveEntity(job, customizer);

            AddSpecialityWithAllData();

            return this;
        }

        public TestDataUtil AddSpeciality(Action<Speciality> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateSpeciality(GetLast<Job>()), customizer);
        }

        public TestDataUtil AddSpecialityWithAllData(Action<Speciality> customizer = null)
        {
            var suffix = RngUtil.GetRandomHexString(8);

            var job = GetLast<Job>();
            var speciality = _defaultEntityCreator.CreateSpeciality(job, suffix);

            speciality.Modifiers = new List<SpecialityModifier>
            {
                new SpecialityModifier
                {
                    Stat = GetLast<Stat>().Name,
                    Value = 2
                }
            };

            speciality.Specials = new List<SpecialitySpecial>
            {
                new SpecialitySpecial
                {
                    Description = $"some-speciality-special-description-{suffix}",
                    Flags = @"[{""data"": ""some-data"", ""type"": ""ONE_SPECIALITY""}]",
                    IsBonus = true,
                }
            };
            return SaveEntity(speciality, customizer);
        }
    }
}