using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Socolin.TestUtils.Console;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class InitializersProfiler
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

    private readonly ISpecFlowOutputHelper _specFlowOutputHelper;
    private static InitProfiler _profiler;
    private static readonly Stopwatch InitStopwatch = new();

    public InitializersProfiler(ISpecFlowOutputHelper specFlowOutputHelper)
    {
        _specFlowOutputHelper = specFlowOutputHelper;
    }

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
        _specFlowOutputHelper.WriteLine($"┌{"".PadRight(titleColumnLength + 2, '─')}─{"".PadRight(14, '─')}┐");
        _specFlowOutputHelper.WriteLine("│" + " Init profiler".PadRight(titleColumnLength + 17, ' ') + "│");
        _specFlowOutputHelper.WriteLine($"├{"".PadRight(titleColumnLength + 2, '─')}┬{"".PadRight(14, '─')}┤");
        var total = _profiler.Sections.Sum(x => x.Stopwatch.ElapsedMilliseconds);
        var minDuration = _profiler.Sections.Min(x => x.Stopwatch.ElapsedMilliseconds);
        var maxDuration = _profiler.Sections.Max(x => x.Stopwatch.ElapsedMilliseconds);
        var durationDiff = (float)(maxDuration - minDuration);
        foreach (var profilerSection in _profiler.Sections)
        {
            var duration = profilerSection.Stopwatch.ElapsedMilliseconds;
            var colorIndex = (int)Math.Floor((GradiantGreenRed.Length - 1) * ((float)duration - minDuration) / durationDiff);
            _specFlowOutputHelper.WriteLine("│ " + profilerSection.Title.PadRight(titleColumnLength, ' ') + " │ " + AnsiColorCodes.Color256(duration.ToString("N0").PadLeft(9) + " ms", GradiantGreenRed[colorIndex]) + " │");
        }
        _specFlowOutputHelper.WriteLine($"├{"".PadRight(titleColumnLength + 2, '─')}┴{"".PadRight(14, '─')}┤");

        _specFlowOutputHelper.WriteLine("│" + " Total: " + InitStopwatch.ElapsedMilliseconds.ToString("N0").PadLeft(titleColumnLength + 5, ' ') + " ms " + "│");
        _specFlowOutputHelper.WriteLine("│" + " Not profiled: " + (InitStopwatch.ElapsedMilliseconds - total).ToString("N0").PadLeft(titleColumnLength - 2, ' ') + " ms " + "│");
        _specFlowOutputHelper.WriteLine($"└{"".PadRight(titleColumnLength + 2, '─')}─{"".PadRight(14, '─')}┘");
    }

    private class InitProfiler
    {
        public IList<ProfileSection> Sections { get; set; } = new List<ProfileSection>();

        public class ProfileSection : IDisposable
        {
            public readonly string Title;
            public readonly Stopwatch Stopwatch;

            public ProfileSection(string title)
            {
                Title = title;
                Stopwatch = Stopwatch.StartNew();
            }

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