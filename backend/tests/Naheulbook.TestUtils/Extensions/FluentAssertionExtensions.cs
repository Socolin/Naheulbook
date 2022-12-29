using System;
using System.Linq;
using FluentAssertions.Equivalency;

namespace Naheulbook.TestUtils.Extensions;

public static class FluentAssertionExtensions
{
    public static EquivalencyAssertionOptions<TExpectation> ExcludingChildren<TExpectation>(this EquivalencyAssertionOptions<TExpectation> config)
    {
        return config
            .ExcludingFields()
            .Including(x => (
                                !x.Path.Contains(".")
                                || (x.Path.StartsWith("[") && x.Path.Count(c => c == '.') == 1)
                            )
                            && (
                                x.Type == typeof(string)
                                || x.Type == typeof(bool)
                                || x.Type == typeof(short)
                                || x.Type == typeof(int)
                                || x.Type == typeof(long)
                                || x.Type == typeof(ushort)
                                || x.Type == typeof(uint)
                                || x.Type == typeof(ulong)
                                || x.Type == typeof(byte)
                                || x.Type == typeof(sbyte)
                                || x.Type == typeof(char)
                                || x.Type == typeof(float)
                                || x.Type == typeof(double)
                                || x.Type == typeof(decimal)
                                || x.Type == typeof(Guid)
                            )
            );
    }
}