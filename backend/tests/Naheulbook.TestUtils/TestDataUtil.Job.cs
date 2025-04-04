using System;
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
        job = new JobEntity
        {
            Name = RngUtil.GetRandomString("some-job-name"),
            Flags = """[{"type": "value"}]""",
            PlayerDescription = RngUtil.GetRandomString("some-player-description"),
            Information = RngUtil.GetRandomString("some-information"),
            PlayerSummary = RngUtil.GetRandomString("some-player-summary"),
            Data = """{"forOrigin": {"all": {"baseEa": 20, "diceEaLevelUp": 6}}}""",
            IsMagic = true,
        };
        return SaveEntity(job, customizer);
    }

    public TestDataUtil AddJobWithAllData(Action<JobEntity> customizer = null)
    {
        AddJob()
            .AddStat().AddJobRequirement(x =>
                {
                    x.MinValue = 2;
                    x.MaxValue = 4;
                }
            )
            .AddJobBonus()
            .AddSkill().AddJobSkill(x => x.Default = true)
            .AddSkill().AddJobSkill(x => x.Default = false)
            .AddJobRestriction()
            .AddSpecialityWithAllData()
            ;

        return this;
    }

    public TestDataUtil AddSpeciality(Action<SpecialityEntity> customizer = null)
    {
        return AddSpeciality(out _, customizer);
    }

    public TestDataUtil AddSpeciality(out SpecialityEntity speciality, Action<SpecialityEntity> customizer = null)
    {
        var job = GetLast<JobEntity>();

        speciality = new SpecialityEntity
        {
            Name = RngUtil.GetRandomString("some-speciality-name"),
            Description = RngUtil.GetRandomString("some-speciality-description"),
            Flags = """[{"type": "value"}]""",
            JobId = job.Id,
        };

        return SaveEntity(speciality, customizer);
    }

    public TestDataUtil AddSpecialityWithAllData(Action<SpecialityEntity> customizer = null)
    {
        return AddSpeciality()
            .AddStat().AddSpecialityModifier()
            .AddSpecialitySpecial();
    }

    public TestDataUtil AddSpecialitySpecial(Action<SpecialitySpecialEntity> customizer = null)
    {
        return AddSpecialitySpecial(out _, customizer);
    }

    public TestDataUtil AddSpecialitySpecial(out SpecialitySpecialEntity jobBonus, Action<SpecialitySpecialEntity> customizer = null)
    {
        var speciality = GetLast<SpecialityEntity>();

        jobBonus = new SpecialitySpecialEntity
        {
            SpecialityId = speciality.Id,
            Description = RngUtil.GetRandomString("some-speciality-special-description"),
            Flags = """[{"data": "some-data", "type": "ONE_SPECIALITY"}]""",
            IsBonus = true,
        };

        return SaveEntity(jobBonus, customizer);
    }

    public TestDataUtil AddSpecialityModifier(Action<SpecialityModifierEntity> customizer = null)
    {
        return AddSpecialityModifier(out _, customizer);
    }

    public TestDataUtil AddSpecialityModifier(out SpecialityModifierEntity jobBonus, Action<SpecialityModifierEntity> customizer = null)
    {
        var speciality = GetLast<SpecialityEntity>();

        jobBonus = new SpecialityModifierEntity
        {
            SpecialityId = speciality.Id,
            Stat = GetLast<StatEntity>().Name,
            Value = 2,
        };

        return SaveEntity(jobBonus, customizer);
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
            Description = RngUtil.GetRandomString("some-job-bonus-description"),
            Flags = """[{"data": "some-data", "type": "some-type"}]""",
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
            Flags = """[{"data": "some-data", "type": "some-type"}]""",
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