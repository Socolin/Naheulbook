using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Stat;

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