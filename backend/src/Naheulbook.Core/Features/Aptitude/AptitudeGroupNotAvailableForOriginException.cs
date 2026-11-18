namespace Naheulbook.Core.Features.Aptitude;

[Serializable]
public class AptitudeGroupNotAvailableForOriginException(Guid aptitudeGroupId) : Exception
{
    public Guid AptitudeGroupId { get; } = aptitudeGroupId;
}
