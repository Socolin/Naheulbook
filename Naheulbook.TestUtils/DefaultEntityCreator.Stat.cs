using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Stat CreateStat(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Stat
            {
                Name = $"some-stat-name-{suffix}",
                Description = $"some-stat-description-{suffix}",
                DisplayName = $"some-stat-display-name-{suffix}"
            };
        }
    }
}