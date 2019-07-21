using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddOrigin(Action<Origin> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateOrigin(), customizer);
        }

        public TestDataUtil AddOriginWithAllData(Action<Origin> customizer = null)
        {
            var suffix = RngUtil.GetRandomHexString(8);

            var origin = _defaultEntityCreator.CreateOrigin(suffix);

            var stat = AddStat().GetLast<Stat>();

            var skill1 = AddSkill().GetLast<Skill>();
            var skill2 = AddSkill().GetLast<Skill>();

            origin.Information.Add(new OriginInfo
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
            origin.Requirements = new List<OriginRequirement>
            {
                new OriginRequirement
                {
                    MaxValue = 5,
                    MinValue = 2,
                    Stat = stat
                }
            };
            origin.Skills = new List<OriginSkill>
            {
                new OriginSkill
                {
                    Skill = skill1,
                    SkillId = skill1.Id,
                    Default = true
                },
                new OriginSkill
                {
                    Skill = skill2,
                    SkillId = skill2.Id,
                    Default = false
                }
            };
            origin.Restrictions = new List<OriginRestrict>
            {
                new OriginRestrict
                {
                    Text = "some-restriction-text",
                    Flags = @"[{""data"": null, ""type"": ""value""}]"
                }
            };

            SaveEntity(origin, customizer);

            return this;
        }

        public TestDataUtil AddOriginRandomNameUrl(Action<OriginRandomNameUrl> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateOriginRandomNameUrl(GetLast<Origin>(), "some-sex"), customizer);
        }
    }
}