using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Talent;

public interface ITalentService
{
    Task<List<TalentGroupEntity>> GetTalentGroupsAsync(CancellationToken cancellationToken = default);
}

public class TalentService(IUnitOfWorkFactory unitOfWorkFactory) : ITalentService
{
    public async Task<List<TalentGroupEntity>> GetTalentGroupsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.TalentGroups.GetAllWithTalentsAsync(cancellationToken);
    }
}
