using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddSkill(Action<SkillEntity> customizer = null)
    {
        return AddSkill(out _, customizer);
    }

    public TestDataUtil AddSkill(out SkillEntity skill, Action<SkillEntity> customizer = null)
    {
        skill = _defaultEntityCreator.CreateSkill();
        return SaveEntity(skill, customizer);
    }
}