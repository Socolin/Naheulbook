namespace Naheulbook.Core.Features.Aptitude;

public class AptitudeNotFoundException(Guid aptitudeId) : Exception
{
    public Guid AptitudeId { get; } = aptitudeId;
}