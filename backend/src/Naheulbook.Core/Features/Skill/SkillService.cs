using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Skill;

public interface ISkillService
{
    Task<ICollection<SkillEntity>> GetSkillsAsync(CancellationToken cancellationToken = default);
}

public class SkillService(IUnitOfWorkFactory unitOfWorkFactory) : ISkillService
{
    public async Task<ICollection<SkillEntity>> GetSkillsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Skills.GetAllWithEffectsAsync(cancellationToken);
    }
}