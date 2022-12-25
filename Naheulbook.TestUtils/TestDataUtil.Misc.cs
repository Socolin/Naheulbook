using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddGod(Action<GodEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGod(), customizer);
        }

        public TestDataUtil AddCalendarEntry(Action<CalendarEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateCalendar(), customizer);
        }
    }
}