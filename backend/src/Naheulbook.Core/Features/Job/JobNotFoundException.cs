

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Job;

public class JobNotFoundException(Guid jobId) : Exception
{
    public Guid JobId { get; } = jobId;
}