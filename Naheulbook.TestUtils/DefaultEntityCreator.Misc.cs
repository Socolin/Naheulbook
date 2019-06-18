using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public God CreateGod(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new God
            {
                TechName = $"some-tech-username-{suffix}",
                DisplayName = $"some-display-name-{suffix}",
                Description = $"some-description-{suffix}"
            };
        }
    }
}