using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

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