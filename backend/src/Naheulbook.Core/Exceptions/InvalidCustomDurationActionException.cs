using System;

namespace Naheulbook.Core.Exceptions;

[Serializable]
public class InvalidCustomDurationActionException(string customDurationType) : Exception
{
    // ReSharper disable once UnusedMember.Local
    private string CustomDurationType { get; } = customDurationType;
}