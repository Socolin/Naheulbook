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
        Task<EffectType> CreateEffectTypeAsync(string effectTypeName);
        Task<EffectCategory> CreateEffectCategoryAsync(string effectCategoryName, int typeId, short diceSize, short diceCount, string note);
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

        public async Task<EffectType> CreateEffectTypeAsync(string effectTypeName)
        {
            var effectType = new EffectType
            {
                Name = effectTypeName
            };

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.EffectTypes.Add(effectType);
                await uow.CompleteAsync();
            }

            return effectType;
        }

        public async Task<EffectCategory> CreateEffectCategoryAsync(string effectCategoryName, int typeId, short diceSize, short diceCount, string note)
        {
            var effectCategory = new EffectCategory
            {
                Name = effectCategoryName,
                TypeId = typeId,
                DiceSize = diceSize,
                DiceCount = diceCount,
                Note = note
            };

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.EffectCategories.Add(effectCategory);
                await uow.CompleteAsync();
            }

            return effectCategory;
        }
    }
}