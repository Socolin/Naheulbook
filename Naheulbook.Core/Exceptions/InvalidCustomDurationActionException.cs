using System;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Naheulbook.Core.Exceptions;

public class InvalidCustomDurationActionException : Exception
{
    private string CustomDurationType { get; }

    public InvalidCustomDurationActionException(string customDurationType)
    {
        CustomDurationType = customDurationType;
    }
}