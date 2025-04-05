using System;
using System.Collections.Generic;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddSkill(Action<SkillEntity> customizer = null)
    {
        return AddSkill(out _, customizer);
    }

    public TestDataUtil AddSkill(out SkillEntity skill, Action<SkillEntity> customizer = null)
    {
        skill = new SkillEntity
        {
            Name = RngUtil.GetRandomString("some-skill-name"),
            Description = RngUtil.GetRandomString("some-skill-description"),
            Flags = """[{"type": "value"}]""",
            PlayerDescription = RngUtil.GetRandomString("some-player-description"),
            Require = RngUtil.GetRandomString("some-require"),
            Roleplay = RngUtil.GetRandomString("some-roleplay"),
            Resist = RngUtil.GetRandomString("some-resist"),
            Using = RngUtil.GetRandomString("some-using"),
            Stat = "FO",
            Test = 2,
            SkillEffects = new List<SkillEffectEntity>
            {
                new()
                {
                    Value = 5,
                    StatName = "CHA",
                },
            },
        };

        return SaveEntity(skill, customizer);
    }
}