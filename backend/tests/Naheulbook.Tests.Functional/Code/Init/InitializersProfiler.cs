using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Socolin.TestUtils.Console;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class InitializersProfiler(ISpecFlowOutputHelper specFlowOutputHelper)
{
    private static readonly Terminal256ColorCodes[] GradiantGreenRed =
    {
        Terminal256ColorCodes.PaleGreen3C77,
        Terminal256ColorCodes.DarkSeaGreen4C71,
        Terminal256ColorCodes.Chartreuse4C64,
        Terminal256ColorCodes.Gold3C178,
        Terminal256ColorCodes.Orange3C172,
        Terminal256ColorCodes.DarkOrange3C130,
        Terminal256ColorCodes.DarkOrange3C166,
        Terminal256ColorCodes.Red3C160,
    };

    private static InitProfiler _profiler;
    private static readonly Stopwatch InitStopwatch = new();

    [BeforeTestRun(Order = 0)]
    public static void SetupProfiler()
    {
        InitStopwatch.Start();
        _profiler = new InitProfiler();
    }

    [BeforeTestRun(Order = int.MaxValue)]
    public static void ComputeTotalInitTime()
    {
        InitStopwatch.Stop();
    }

    public static IDisposable Profile(string title)
    {
        return _profiler.StartProfile(title);
    }

    [AfterScenario(Order = int.MaxValue)]
    public void RegisterServiceInformation()
    {
        var titleColumnLength = _profiler.Sections.Max(x => x.Title.Length);
        specFlowOutputHelper.WriteLine($"┌{"".PadRight(titleColumnLength + 2, '─')}─{"".PadRight(14, '─')}┐");
        specFlowOutputHelper.WriteLine("│" + " Init profiler".PadRight(titleColumnLength + 17, ' ') + "│");
        specFlowOutputHelper.WriteLine($"├{"".PadRight(titleColumnLength + 2, '─')}┬{"".PadRight(14, '─')}┤");
        var total = _profiler.Sections.Sum(x => x.Stopwatch.ElapsedMilliseconds);
        var minDuration = _profiler.Sections.Min(x => x.Stopwatch.ElapsedMilliseconds);
        var maxDuration = _profiler.Sections.Max(x => x.Stopwatch.ElapsedMilliseconds);
        var durationDiff = (float)(maxDuration - minDuration);
        foreach (var profilerSection in _profiler.Sections)
        {
            var duration = profilerSection.Stopwatch.ElapsedMilliseconds;
            var colorIndex = (int)Math.Floor((GradiantGreenRed.Length - 1) * ((float)duration - minDuration) / durationDiff);
            specFlowOutputHelper.WriteLine("│ " + profilerSection.Title.PadRight(titleColumnLength, ' ') + " │ " + AnsiColorCodes.Color256(duration.ToString("N0").PadLeft(9) + " ms", GradiantGreenRed[colorIndex]) + " │");
        }
        specFlowOutputHelper.WriteLine($"├{"".PadRight(titleColumnLength + 2, '─')}┴{"".PadRight(14, '─')}┤");

        specFlowOutputHelper.WriteLine("│" + " Total: " + InitStopwatch.ElapsedMilliseconds.ToString("N0").PadLeft(titleColumnLength + 5, ' ') + " ms " + "│");
        specFlowOutputHelper.WriteLine("│" + " Not profiled: " + (InitStopwatch.ElapsedMilliseconds - total).ToString("N0").PadLeft(titleColumnLength - 2, ' ') + " ms " + "│");
        specFlowOutputHelper.WriteLine($"└{"".PadRight(titleColumnLength + 2, '─')}─{"".PadRight(14, '─')}┘");
    }

    private class InitProfiler
    {
        public IList<ProfileSection> Sections { get; set; } = new List<ProfileSection>();

        public class ProfileSection(string title) : IDisposable
        {
            public readonly string Title = title;
            public readonly Stopwatch Stopwatch = Stopwatch.StartNew();

            public void Dispose()
            {
                Stopwatch.Stop();
            }
        }

        public IDisposable StartProfile(string title)
        {
            var profileSection = new ProfileSection(title);
            Sections.Add(profileSection);
            return profileSection;
        }
    }
}