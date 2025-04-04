using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface IOriginService
{
    Task<ICollection<OriginEntity>> GetOriginsAsync();
}

public class OriginService(IUnitOfWorkFactory unitOfWorkFactory) : IOriginService
{
    public async Task<ICollection<OriginEntity>> GetOriginsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Origins.GetAllWithAllDataAsync();
    }
}