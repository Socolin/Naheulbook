using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface IStatService
{
    Task<List<StatEntity>> GetAllStatsAsync();
}

public class StatService : IStatService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public StatService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<List<StatEntity>> GetAllStatsAsync()
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.Stats.GetAllAsync();
        }
    }
}