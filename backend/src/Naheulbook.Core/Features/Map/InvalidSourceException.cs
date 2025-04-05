namespace Naheulbook.Core.Features.Map;

[Serializable]
public class InvalidSourceException(string source) : Exception
{
    public string SourceValue { get; } = source;
}