using System;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Naheulbook.Core.Exceptions;

public class InvalidCustomDurationActionException(string customDurationType) : Exception
{
    private string CustomDurationType { get; } = customDurationType;
}