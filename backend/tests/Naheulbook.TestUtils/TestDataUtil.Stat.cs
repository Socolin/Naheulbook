using System;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddStat(Action<StatEntity> customizer = null)
    {
        return AddStat(out _, customizer);
    }

    public TestDataUtil AddStat(out StatEntity stat, Action<StatEntity> customizer = null)
    {
        var suffix = RngUtil.GetRandomHexString(8);

        stat = new StatEntity
        {
            Name = $"some-stat-name-{suffix}",
            Description = $"some-stat-description-{suffix}",
            DisplayName = $"some-stat-display-name-{suffix}",
        };

        return SaveEntity(stat, customizer);
    }
}