using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface IEffectService
    {
        Task<ICollection<EffectType>> GetEffectCategoriesAsync();
        Task<ICollection<Effect>> GetEffectsByCategoryAsync(long categoryId);
    }

    public class EffectService : IEffectService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public EffectService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<ICollection<EffectType>> GetEffectCategoriesAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Effects.GetCategoriesAsync();
            }
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