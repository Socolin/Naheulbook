namespace Naheulbook.Shared.Utils;

public struct DoNotInterrupt
{
    public static CancellationToken Token => default;
}