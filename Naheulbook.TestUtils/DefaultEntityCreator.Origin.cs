using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public OriginEntity CreateOrigin(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new OriginEntity
            {
                Name = $"some-origin-name-{suffix}",
                Flags = @"[{""type"": ""value""}]",
                PlayerDescription = $"some-player-description-{suffix}",
                Description = $"some-description-{suffix}",
                PlayerSummary = $"some-player-summary-{suffix}",
                Advantage = $"some-advantage-{suffix}",
                Data = @"{""baseEv"": 20, ""maxLoad"": 10, ""maxArmorPr"": 2, ""diceEvLevelUp"": 4, ""speedModifier"": -20}",
                Size = $"some-size-{suffix}",
                Bonuses = new List<OriginBonus>(),
                Information = new List<OriginInfoEntity>(),
                Requirements = new List<OriginRequirementEntity>(),
                Restrictions = new List<OriginRestrictEntity>(),
                Skills = new List<OriginSkillEntity>()
            };
        }

        public OriginRandomNameUrlEntity CreateOriginRandomNameUrl(OriginEntity origin, string sex)
        {
            return new OriginRandomNameUrlEntity
            {
                Origin = origin,
                Sex = sex,
                Url = "/generateurs/noms/naheulbeuk/sex/originname"
            };
        }
    }
}