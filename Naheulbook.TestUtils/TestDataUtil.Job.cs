using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddJob(Action<JobEntity> customizer = null)
        {
            return AddJob(out _, customizer);
        }

        public TestDataUtil AddJob(out JobEntity job, Action<JobEntity> customizer = null)
        {
            job = _defaultEntityCreator.CreateJob();
            return SaveEntity(job, customizer);
        }

        public TestDataUtil AddJobWithAllData(Action<JobEntity> customizer = null)
        {
            var suffix = RngUtil.GetRandomHexString(8);

            var job = _defaultEntityCreator.CreateJob(suffix);

            var stat = AddStat().GetLast<StatEntity>();

            var skill1 = AddSkill().GetLast<SkillEntity>();
            var skill2 = AddSkill().GetLast<SkillEntity>();

            job.Bonuses = new List<JobBonus>
            {
                new JobBonus
                {
                    Description = $"some-job-bonus-description-{suffix}"
                }
            };
            job.Requirements = new List<JobRequirementEntity>
            {
                new JobRequirementEntity
                {
                    Stat = stat,
                    MinValue = 2,
                    MaxValue = 4,
                }
            };
            job.Restrictions = new List<JobRestrictionEntity>
            {
                new JobRestrictionEntity
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

            SaveEntity(job, customizer);

            AddSpecialityWithAllData();

            return this;
        }

        public TestDataUtil AddSpeciality(Action<SpecialityEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateSpeciality(GetLast<JobEntity>()), customizer);
        }

        public TestDataUtil AddSpecialityWithAllData(Action<SpecialityEntity> customizer = null)
        {
            var suffix = RngUtil.GetRandomHexString(8);

            var job = GetLast<JobEntity>();
            var speciality = _defaultEntityCreator.CreateSpeciality(job, suffix);

            speciality.Modifiers = new List<SpecialityModifierEntity>
            {
                new SpecialityModifierEntity
                {
                    Stat = GetLast<StatEntity>().Name,
                    Value = 2
                }
            };

            speciality.Specials = new List<SpecialitySpecialEntity>
            {
                new SpecialitySpecialEntity
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