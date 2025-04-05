using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Origin;

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