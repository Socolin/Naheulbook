using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions;

public class JobNotFoundException : Exception
{
    public Guid JobId { get; }

    public JobNotFoundException(Guid jobId)
    {
        JobId = jobId;
    }
}