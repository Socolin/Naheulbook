using System;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CharacterAddJobRequest
{
    public Guid JobId { get; set; }
}