using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Handicap;

public interface IHandicapService
{
    Task<List<HandicapEntity>> GetHandicapsAsync(CancellationToken cancellationToken = default);
}

public class HandicapService(IUnitOfWorkFactory unitOfWorkFactory) : IHandicapService
{
    public async Task<List<HandicapEntity>> GetHandicapsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Handicaps.GetAllOrderedByDiceRollAsync(cancellationToken);
    }
}
