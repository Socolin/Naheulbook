using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddOrigin(Action<OriginEntity> customizer = null)
    {
        return AddOrigin(out _, customizer);
    }

    public TestDataUtil AddOrigin(out OriginEntity origin, Action<OriginEntity> customizer = null)
    {
        var aptitudeGroup = GetLastIfExists<AptitudeGroupEntity>();
        if (aptitudeGroup is null)
            AddAptitudeGroup(out aptitudeGroup);
        origin = new OriginEntity
        {
            Name = RngUtil.GetRandomString("some-origin-name"),
            Flags = @"[{""type"": ""value""}]",
            PlayerDescription = RngUtil.GetRandomString("some-player-description"),
            Description = RngUtil.GetRandomString("some-description"),
            PlayerSummary = RngUtil.GetRandomString("some-player-summary"),
            Advantage = RngUtil.GetRandomString("some-advantage"),
            Data = @"{""baseEv"": 20, ""maxLoad"": 10, ""maxArmorPr"": 2, ""diceEvLevelUp"": 4, ""speedModifier"": -20}",
            Size = RngUtil.GetRandomString("some-size"),
            Bonuses = [],
            Information = [],
            Requirements = [],
            Restrictions = [],
            Skills = [],
            AptitudeGroupId = aptitudeGroup.Id,
        };
        return SaveEntity(origin, customizer);
    }

    public TestDataUtil AddOriginWithAllData(Action<OriginEntity> customizer = null)
    {
        AddOrigin()
            .AddSkill().AddOriginSkill(x => x.Default = true)
            .AddSkill().AddOriginSkill(x => x.Default = false)
            .AddOriginBonus()
            .AddOriginInfo()
            .AddStat().AddOriginRequirement()
            .AddOriginRestrict()
            ;

        return this;
    }

    public TestDataUtil AddOriginRestrict(Action<OriginRestrictEntity> customizer = null)
    {
        return AddOriginRestrict(out _, customizer);
    }

    public TestDataUtil AddOriginRestrict(out OriginRestrictEntity originSkill, Action<OriginRestrictEntity> customizer = null)
    {
        originSkill = new OriginRestrictEntity
        {
            OriginId = GetLast<OriginEntity>().Id,
            Text = "some-restriction-text",
            Flags = """[{"data": null, "type": "value"}]""",
        };

        return SaveEntity(originSkill, customizer);
    }

    public TestDataUtil AddOriginRequirement(Action<OriginRequirementEntity> customizer = null)
    {
        return AddOriginRequirement(out _, customizer);
    }

    public TestDataUtil AddOriginRequirement(out OriginRequirementEntity originSkill, Action<OriginRequirementEntity> customizer = null)
    {
        originSkill = new OriginRequirementEntity
        {
            OriginId = GetLast<OriginEntity>().Id,
            MaxValue = 5,
            MinValue = 2,
            StatName = GetLast<StatEntity>().Name,
        };

        return SaveEntity(originSkill, customizer);
    }

    public TestDataUtil AddOriginInfo(Action<OriginInfoEntity> customizer = null)
    {
        return AddOriginInfo(out _, customizer);
    }

    public TestDataUtil AddOriginInfo(out OriginInfoEntity originSkill, Action<OriginInfoEntity> customizer = null)
    {
        originSkill = new OriginInfoEntity
        {
            OriginId = GetLast<OriginEntity>().Id,
            Description = RngUtil.GetRandomString("some-origin-info-description"),
            Title = RngUtil.GetRandomString("some-origin-info-title"),
        };

        return SaveEntity(originSkill, customizer);
    }

    public TestDataUtil AddOriginBonus(Action<OriginBonusEntity> customizer = null)
    {
        return AddOriginBonus(out _, customizer);
    }

    public TestDataUtil AddOriginBonus(out OriginBonusEntity originSkill, Action<OriginBonusEntity> customizer = null)
    {
        originSkill = new OriginBonusEntity
        {
            OriginId = GetLast<OriginEntity>().Id,
            Flags = "[]",
            Description = "some-description",
        };

        return SaveEntity(originSkill, customizer);
    }

    public TestDataUtil AddOriginSkill(Action<OriginSkillEntity> customizer = null)
    {
        return AddOriginSkill(out _, customizer);
    }

    public TestDataUtil AddOriginSkill(out OriginSkillEntity originSkill, Action<OriginSkillEntity> customizer = null)
    {
        originSkill = new OriginSkillEntity
        {
            OriginId = GetLast<OriginEntity>().Id,
            SkillId = GetLast<SkillEntity>().Id,
            Default = true,
        };

        return SaveEntity(originSkill, customizer);
    }

    public TestDataUtil AddOriginRandomNameUrl(Action<OriginRandomNameUrlEntity> customizer = null)
    {
        return AddOriginRandomNameUrl(out _, customizer);
    }

    public TestDataUtil AddOriginRandomNameUrl(out OriginRandomNameUrlEntity originRandomNameUrl, Action<OriginRandomNameUrlEntity> customizer = null)
    {
        var origin = GetLast<OriginEntity>();

        originRandomNameUrl = new OriginRandomNameUrlEntity
        {
            OriginId = origin.Id,
            Sex = "some-sex",
            Url = "/generateurs/noms/naheulbeuk/sex/originname",
        };

        return SaveEntity(originRandomNameUrl, customizer);
    }
}