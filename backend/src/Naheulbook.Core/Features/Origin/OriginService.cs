using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Origin;

public interface IOriginService
{
    Task<ICollection<OriginEntity>> GetOriginsAsync(CancellationToken cancellationToken = default);
}

public class OriginService(IUnitOfWorkFactory unitOfWorkFactory) : IOriginService
{
    public async Task<ICollection<OriginEntity>> GetOriginsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Origins.GetAllWithAllDataAsync(cancellationToken);
    }
}