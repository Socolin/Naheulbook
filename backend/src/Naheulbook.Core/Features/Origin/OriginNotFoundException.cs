namespace Naheulbook.Core.Features.Origin;

[Serializable]
public class OriginNotFoundException(Guid originId) : Exception
{
    public Guid OriginId { get; } = originId;
}