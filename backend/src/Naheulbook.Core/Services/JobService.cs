using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

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