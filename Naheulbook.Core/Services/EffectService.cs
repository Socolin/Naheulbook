using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface IEffectService
    {
        Task<ICollection<Effect>> GetEffectsByCategoryAsync(long categoryId);
    }

    public class EffectService : IEffectService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public EffectService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ICollection<Effect>> GetEffectsByCategoryAsync(long categoryId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Effects.GetByCategoryWithModifiersAsync(categoryId);
            }
        }
    }
}