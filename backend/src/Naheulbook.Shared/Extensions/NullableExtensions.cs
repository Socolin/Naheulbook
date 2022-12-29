using System;
using System.Runtime.CompilerServices;

namespace Naheulbook.Shared.Extensions;

public static class NullableExtensions
{
    public static T NotNull<T>(
        this T? value,
        [CallerArgumentExpression("value")] string? expression = null
    ) where T : class
    {
        if (value == null)
            throw new NullReferenceException($"{expression} should not be null");
        return value;
    }
}