namespace Naheulbook.Core.Features.Aptitude;

public class AptitudeGroupNotFoundException(Guid aptitudeGroupId) : Exception
{
    public Guid AptitudeGroupId { get; } = aptitudeGroupId;
}