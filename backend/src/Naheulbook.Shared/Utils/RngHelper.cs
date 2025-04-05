using System.Security.Cryptography;
using System.Text;

namespace Naheulbook.Shared.Utils;

public class RngHelper
{
    public static string GetRandomHexString(int byteCount)
    {
        var bytes = new byte[byteCount];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(bytes);
        var sb = new StringBuilder();
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public static string GetRandomString(string prefix)
    {
        return prefix + GetRandomHexString(8);
    }

    public static int GetRandomInt()
    {
        var bytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes);
    }

    public static int GetRandomInt(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentOutOfRangeException(nameof(minValue));
        if (minValue == maxValue)
            return minValue;

        const long max = 1 + (long)uint.MaxValue;
        var diff = maxValue - minValue;
        var remainder = max % diff;

        var bytes = new byte[4];
        using var rng = RandomNumberGenerator.Create();
        while (true)
        {
            rng.GetBytes(bytes);
            var number = BitConverter.ToUInt32(bytes);
            if (number < max - remainder)
                return (int)(minValue + (number % diff));
        }
    }
}