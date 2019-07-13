using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Location CreateLocation(Location parent = null, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Location
            {
                Name = $"some-name-{suffix}",
                Data = "{}",
                Parent = parent
            };
        }
    }
}