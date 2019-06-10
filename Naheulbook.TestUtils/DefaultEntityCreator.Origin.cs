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
                BaseEa = 10,
                BaseEv = 12,
                MaxLoad = 15,
                MaxArmorPr = 6,
                Size = $"some-size-{suffix}",
                BonusAt = 2,
                BonusPrd = 3,
                SpeedModifier = 12,
                DiceEvLevelUp = 2,
            };
        }
    }
}