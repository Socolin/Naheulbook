using Socolin.TestUtils.AutoFillTestObjects.Utils;

namespace Socolin.TestUtils.AutoFillTestObjects;

internal class AutoFillContext
{
    public AutoFillFlags Flags { get; set; }
    public int IntValue { get; set; }
    public AutoFillSettings Settings { get; set; }
    public int Depth => Levels.Count;
    public HashSet<string> IgnoredMembers { get; set; }
    public List<string> Levels { get; } = new();

    public int GetNextInt()
    {
        return Flags.HasFlag(AutoFillFlags.RandomInt) ? RngUtils.GetRandomInt() : IntValue++;
    }
}