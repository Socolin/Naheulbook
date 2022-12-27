using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddOrigin(Action<OriginEntity> customizer = null)
    {
        return AddOrigin(out _, customizer);
    }

    public TestDataUtil AddOrigin(out OriginEntity origin, Action<OriginEntity> customizer = null)
    {
        origin = _defaultEntityCreator.CreateOrigin();
        return SaveEntity(origin, customizer);
    }

    public TestDataUtil AddOriginWithAllData(Action<OriginEntity> customizer = null)
    {
        var suffix = RngUtil.GetRandomHexString(8);

        var origin = _defaultEntityCreator.CreateOrigin(suffix);

        var stat = AddStat().GetLast<StatEntity>();

        var skill1 = AddSkill().GetLast<SkillEntity>();
        var skill2 = AddSkill().GetLast<SkillEntity>();

        origin.Information.Add(new OriginInfoEntity
        {
            Description = $"some-origin-info-description-{suffix}",
            Title = $"some-origin-info-title-{suffix}"
        });
        origin.Bonuses = new List<OriginBonus>
        {
            new OriginBonus
            {
                Flags = "[]",
                Description = $"some-description-{suffix}"
            }
        };
        origin.Requirements = new List<OriginRequirementEntity>
        {
            new OriginRequirementEntity
            {
                MaxValue = 5,
                MinValue = 2,
                Stat = stat
            }
        };
        origin.Skills = new List<OriginSkillEntity>
        {
            new OriginSkillEntity
            {
                Skill = skill1,
                SkillId = skill1.Id,
                Default = true
            },
            new OriginSkillEntity
            {
                Skill = skill2,
                SkillId = skill2.Id,
                Default = false
            }
        };
        origin.Restrictions = new List<OriginRestrictEntity>
        {
            new OriginRestrictEntity
            {
                Text = "some-restriction-text",
                Flags = @"[{""data"": null, ""type"": ""value""}]"
            }
        };

        SaveEntity(origin, customizer);

        return this;
    }

    public TestDataUtil AddOriginRandomNameUrl(Action<OriginRandomNameUrlEntity> customizer = null)
    {
        return SaveEntity(_defaultEntityCreator.CreateOriginRandomNameUrl(GetLast<OriginEntity>(), "some-sex"), customizer);
    }
}