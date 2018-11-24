using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface ISkillService
    {
        Task<ICollection<Skill>> GetSkillsAsync();
    }

    public class SkillService : ISkillService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SkillService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ICollection<Skill>> GetSkillsAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Skills.GetAllWithEffectsAsync();
            }
        }
    }
}