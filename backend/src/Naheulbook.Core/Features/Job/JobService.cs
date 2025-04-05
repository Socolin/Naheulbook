using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Job;

public interface IJobService
{
    Task<ICollection<JobEntity>> GetJobsAsync();
}

public class JobService(IUnitOfWorkFactory unitOfWorkFactory) : IJobService
{
    public async Task<ICollection<JobEntity>> GetJobsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Jobs.GetAllWithAllDataAsync();
    }
}