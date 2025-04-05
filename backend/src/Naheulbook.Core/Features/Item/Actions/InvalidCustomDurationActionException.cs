namespace Naheulbook.Core.Features.Item.Actions;

[Serializable]
public class InvalidCustomDurationActionException(string customDurationType) : Exception
{
    // ReSharper disable once UnusedMember.Local
    private string CustomDurationType { get; } = customDurationType;
}