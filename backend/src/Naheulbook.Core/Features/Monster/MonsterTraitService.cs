using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterTraitService
{
    Task<List<MonsterTraitEntity>> GetMonsterTraitsAsync();
}

public class MonsterTraitService(IUnitOfWorkFactory unitOfWorkFactory) : IMonsterTraitService
{
    public async Task<List<MonsterTraitEntity>> GetMonsterTraitsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.MonsterTraits.GetAllAsync();
    }
}