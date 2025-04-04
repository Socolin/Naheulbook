using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface IStatService
{
    Task<List<StatEntity>> GetAllStatsAsync();
}

public class StatService(IUnitOfWorkFactory unitOfWorkFactory) : IStatService
{
    public async Task<List<StatEntity>> GetAllStatsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Stats.GetAllAsync();
    }
}