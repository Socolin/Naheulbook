using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Exceptions
{
    public class JobNotFoundException : Exception
    {
        public int JobId { get; }

        public JobNotFoundException(int jobId)
        {
            JobId = jobId;
        }
    }
}