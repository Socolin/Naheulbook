using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Group;

public class NhbkDate
{
    public NhbkDate()
    {
    }

    public NhbkDate(NhbkDateRequest request)
    {
        Year = request.Year;
        Day = request.Day;
        Hour = request.Hour;
        Minute = request.Minute;
    }

    public const int YearDuration = 365;
    public int Year { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }

    public void Add(NhbkDateOffset dateOffset)
    {
        Minute += dateOffset.Minute;
        while (Minute >= 60)
        {
            Minute -= 60;
            Hour++;
        }

        Hour += dateOffset.Hour;
        while (Hour >= 24)
        {
            Hour -= 24;
            Day++;
        }

        Day += dateOffset.Day;
        Day += dateOffset.Week * 7;
        while (Day >= YearDuration)
        {
            Day -= YearDuration;
            Year++;
        }

        Year += dateOffset.Year;
    }
}