namespace Naheulbook.Core.Features.Aptitude;

[Serializable]
public class AptitudeNotAvailableForOriginException(Guid aptitudeId) : Exception
{
    public Guid AptitudeId { get; } = aptitudeId;
}