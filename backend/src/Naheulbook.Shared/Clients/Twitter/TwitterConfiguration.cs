// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Shared.Clients.Twitter;

public class TwitterConfiguration
{
    public string AppId { get; set; } = null!;
    public string AppSecret { get; set; } = null!;
    public string Callback { get; set; } = null!;
}