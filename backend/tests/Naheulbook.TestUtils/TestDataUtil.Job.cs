using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddJob(Action<JobEntity> customizer = null)
    {
        return AddJob(out _, customizer);
    }

    public TestDataUtil AddJob(out JobEntity job, Action<JobEntity> customizer = null)
    {
        job = defaultEntityCreator.CreateJob();
        return SaveEntity(job, customizer);
    }

    public TestDataUtil AddJobWithAllData(Action<JobEntity> customizer = null)
    {
        var suffix = RngUtil.GetRandomHexString(8);

        var job = defaultEntityCreator.CreateJob(suffix);

        var stat = AddStat().GetLast<StatEntity>();

        var skill1 = AddSkill().GetLast<SkillEntity>();
        var skill2 = AddSkill().GetLast<SkillEntity>();

        job.Bonuses = new List<JobBonusEntity>
        {
            new JobBonusEntity
            {
                Description = $"some-job-bonus-description-{suffix}",
            },
        };
        job.Requirements = new List<JobRequirementEntity>
        {
            new JobRequirementEntity
            {
                Stat = stat,
                MinValue = 2,
                MaxValue = 4,
            },
        };
        job.Restrictions = new List<JobRestrictionEntity>
        {
            new JobRestrictionEntity
            {
                Text = $"some-job-restriction-{suffix}",
                Flags = "[]",
            },
        };
        job.Skills = new List<JobSkillEntity>
        {
            new JobSkillEntity
            {
                Default = true,
                Skill = skill1,
            },
            new JobSkillEntity
            {
                Default = false,
                Skill = skill2,
            },
        };

        SaveEntity(job, customizer);

        AddSpecialityWithAllData();

        return this;
    }

    public TestDataUtil AddSpeciality(Action<SpecialityEntity> customizer = null)
    {
        return AddSpeciality(out _, customizer);
    }

    public TestDataUtil AddSpeciality(out SpecialityEntity speciality, Action<SpecialityEntity> customizer = null)
    {
        var job = GetLast<JobEntity>();
        speciality = defaultEntityCreator.CreateSpeciality(job);
        return SaveEntity(speciality, customizer);
    }

    public TestDataUtil AddSpecialityWithAllData(Action<SpecialityEntity> customizer = null)
    {
        var suffix = RngUtil.GetRandomHexString(8);

        var job = GetLast<JobEntity>();
        var speciality = defaultEntityCreator.CreateSpeciality(job, suffix);

        speciality.Modifiers = new List<SpecialityModifierEntity>
        {
            new SpecialityModifierEntity
            {
                Stat = GetLast<StatEntity>().Name,
                Value = 2,
            },
        };

        speciality.Specials = new List<SpecialitySpecialEntity>
        {
            new SpecialitySpecialEntity
            {
                Description = $"some-speciality-special-description-{suffix}",
                Flags = @"[{""data"": ""some-data"", ""type"": ""ONE_SPECIALITY""}]",
                IsBonus = true,
            },
        };
        return SaveEntity(speciality, customizer);
    }

    public TestDataUtil AddJobBonus(Action<JobBonusEntity> customizer = null)
    {
        return AddJobBonus(out _, customizer);
    }

    public TestDataUtil AddJobBonus(out JobBonusEntity jobBonus, Action<JobBonusEntity> customizer = null)
    {
        var job = GetLast<JobEntity>();
        jobBonus = new JobBonusEntity
        {
            JobId = job.Id,
            Description = RngUtil.GetRandomString("some-description"),
            Flags = """["some-flag"]""" ,
        };
        return SaveEntity(jobBonus, customizer);
    }

    public TestDataUtil AddJobRequirement(Action<JobRequirementEntity> customizer = null)
    {
        return AddJobRequirement(out _, customizer);
    }

    public TestDataUtil AddJobRequirement(out JobRequirementEntity jobRequirement, Action<JobRequirementEntity> customizer = null)
    {
        var job = GetLast<JobEntity>();
        var stat = GetLast<StatEntity>();
        jobRequirement = new JobRequirementEntity
        {
            JobId = job.Id,
            StatName = stat.Name,
            MaxValue = 10,
            MinValue = 5,
        };
        return SaveEntity(jobRequirement, customizer);
    }

    public TestDataUtil AddJobRestriction(Action<JobRestrictionEntity> customizer = null)
    {
        return AddJobRestriction(out _, customizer);
    }

    public TestDataUtil AddJobRestriction(out JobRestrictionEntity jobRestriction, Action<JobRestrictionEntity> customizer = null)
    {
        var job = GetLast<JobEntity>();
        jobRestriction = new JobRestrictionEntity
        {
            JobId = job.Id,
            Text = RngUtil.GetRandomString("some-text"),
            Flags = """["some-flags"]""",
        };
        return SaveEntity(jobRestriction, customizer);
    }

    public TestDataUtil AddJobSkill(Action<JobSkillEntity> customizer = null)
    {
        var skill = GetLast<SkillEntity>();
        return AddJobSkill(out _, skill, customizer);
    }

    public TestDataUtil AddJobSkill(SkillEntity skill, Action<JobSkillEntity> customizer = null)
    {
        return AddJobSkill(out _, skill, customizer);
    }

    public TestDataUtil AddJobSkill(out JobSkillEntity jobSkill, SkillEntity skill, Action<JobSkillEntity> customizer = null)
    {
        var job = GetLast<JobEntity>();
        jobSkill = new JobSkillEntity
        {
            JobId = job.Id,
            SkillId = skill.Id,
        };
        return SaveEntity(jobSkill, customizer);
    }
}