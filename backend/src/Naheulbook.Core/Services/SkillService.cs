using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface ISkillService
{
    Task<ICollection<SkillEntity>> GetSkillsAsync();
}

public class SkillService(IUnitOfWorkFactory unitOfWorkFactory) : ISkillService
{
    public async Task<ICollection<SkillEntity>> GetSkillsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Skills.GetAllWithEffectsAsync();
    }
}