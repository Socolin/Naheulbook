using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.God;

public interface IGodService
{
    Task<List<GodEntity>> GetAllGodsAsync(CancellationToken cancellationToken = default);
}

public class GodService(IUnitOfWorkFactory unitOfWorkFactory) : IGodService
{
    public async Task<List<GodEntity>> GetAllGodsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Gods.GetAllAsync(cancellationToken);
    }
}