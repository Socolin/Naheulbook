using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Origin CreateOrigin(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Origin
            {
                Name = $"some-origin-name-{suffix}",
                Flags = @"[{""type"": ""value""}]",
                PlayerDescription = $"some-player-description-{suffix}",
                Description = $"some-description-{suffix}",
                PlayerSummary = $"some-player-summary-{suffix}",
                Advantage = $"some-advantage-{suffix}",
                Data = @"{""baseEv"": 20, ""maxLoad"": 10, ""maxArmorPr"": null, ""diceEvLevelUp"": 4, ""speedModifier"": -20}",
                Size = $"some-size-{suffix}",
                Bonuses = new List<OriginBonus>(),
                Information = new List<OriginInfo>(),
                Requirements = new List<OriginRequirement>(),
                Restrictions = new List<OriginRestrict>(),
                Skills = new List<OriginSkill>()
            };
        }

        public OriginRandomNameUrl CreateOriginRandomNameUrl(Origin origin, string sex)
        {
            return new OriginRandomNameUrl
            {
                Origin = origin,
                Sex = sex,
                Url = "/generateurs/noms/naheulbeuk/sex/originname"
            };
        }
    }
}