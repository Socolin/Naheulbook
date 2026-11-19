using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Mastery;

public interface IMasteryService
{
    Task<List<MasteryGroupEntity>> GetMasteryGroupsAsync(CancellationToken cancellationToken = default);
}

public class MasteryService(IUnitOfWorkFactory unitOfWorkFactory) : IMasteryService
{
    public async Task<List<MasteryGroupEntity>> GetMasteryGroupsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.MasteryGroups.GetAllWithMasteriesAsync(cancellationToken);
    }
}
