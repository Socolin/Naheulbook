using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils;

public partial class DefaultEntityCreator
{
    public SkillEntity CreateSkill(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new SkillEntity
        {
            Name = $"some-skill-name-{suffix}",
            Description = $"some-skill-description-{suffix}",
            Flags = @"[{""type"": ""value""}]",
            PlayerDescription = $"some-player-description-{suffix}",
            Require = $"some-require-{suffix}",
            Roleplay = $"some-roleplay-{suffix}",
            Resist = $"some-resist-{suffix}",
            Using = $"some-using-{suffix}",
            Stat = "FO",
            Test = 2,
            SkillEffects = new List<SkillEffect>
            {
                new SkillEffect
                {
                    Value = 5,
                    StatName = "CHA"
                }
            }
        };
    }
}