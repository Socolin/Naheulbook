using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface IJobService
    {
        Task<ICollection<Job>> GetJobsAsync();
    }

    public class JobService : IJobService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public JobService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ICollection<Job>> GetJobsAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Jobs.GetAllWithAllDataAsync();
            }
        }
    }
}