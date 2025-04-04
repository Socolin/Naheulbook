namespace Naheulbook.Core.Features.Group;

public class NhbkDateOffset
{
    public const int YearDuration = 365 * 24 * 3600;
    public const int WeekDuration = 7 * 24 * 3600;
    public const int DayDuration = 24 * 3600;
    public const int HourDuration = 3600;
    public const int MinuteDuration = 60;

    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Week { get; set; }
    public int Year { get; set; }

    public int GetDuration()
    {
        return Year * YearDuration
               + Week * WeekDuration
               + Day * DayDuration
               + Hour * HourDuration
               + Minute * MinuteDuration;
    }
}