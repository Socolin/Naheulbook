using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddStat(Action<StatEntity> customizer = null)
    {
        return AddStat(out _, customizer);
    }

    public TestDataUtil AddStat(out StatEntity stat, Action<StatEntity> customizer = null)
    {
        stat = defaultEntityCreator.CreateStat();
        return SaveEntity(stat, customizer);
    }
}