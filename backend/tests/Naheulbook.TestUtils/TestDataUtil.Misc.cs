using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddGod(Action<GodEntity> customizer = null)
    {
        return AddGod(out _, customizer);
    }

    public TestDataUtil AddGod(out GodEntity god, Action<GodEntity> customizer = null)
    {
        god = new GodEntity
        {
            TechName = RngUtil.GetRandomString("some-tech-name"),
            DisplayName = RngUtil.GetRandomString("some-display-name"),
            Description = RngUtil.GetRandomString("some-description"),
        };

        return SaveEntity(god, customizer);
    }

    public TestDataUtil AddCalendarEntry(Action<CalendarEntity> customizer = null)
    {
        return AddCalendarEntry(out _, customizer);
    }

    public TestDataUtil AddCalendarEntry(out CalendarEntity calendar, Action<CalendarEntity> customizer = null)
    {
        calendar = new CalendarEntity
        {
            Name = RngUtil.GetRandomString("some-name"),
            Note = "some-note",
            StartDay = 1,
            EndDay = 10,
        };
        return SaveEntity(calendar, customizer);
    }
}