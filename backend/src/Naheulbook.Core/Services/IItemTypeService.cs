using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface IItemTypeService
{
    Task<List<ItemTypeEntity>> GetAllItemTypesAsync();
}

public class ItemTypeService(IUnitOfWorkFactory unitOfWorkFactory) : IItemTypeService
{
    public async Task<List<ItemTypeEntity>> GetAllItemTypesAsync()
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ItemTypes.GetAllAsync();
        }
    }
}