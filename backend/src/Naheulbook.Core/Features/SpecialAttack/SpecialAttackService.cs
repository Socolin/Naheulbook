using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.SpecialAttack;

public interface ISpecialAttackService
{
    Task<List<SpecialAttackGroupEntity>> GetSpecialAttackGroupsAsync(CancellationToken cancellationToken = default);
}

public class SpecialAttackService(IUnitOfWorkFactory unitOfWorkFactory) : ISpecialAttackService
{
    public async Task<List<SpecialAttackGroupEntity>> GetSpecialAttackGroupsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.SpecialAttackGroups.GetAllWithSpecialAttacksAsync(cancellationToken);
    }
}
