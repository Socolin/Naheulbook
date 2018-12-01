using System;

namespace Naheulbook.Shared.Utils
{
    public interface ITimeService
    {
        DateTimeOffset UtcNow { get; }
    }

    public class TimeService : ITimeService
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}