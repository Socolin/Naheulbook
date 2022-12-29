using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface IItemTypeService
{
    Task<List<ItemTypeEntity>> GetAllItemTypesAsync();
}

public class ItemTypeService : IItemTypeService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ItemTypeService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<List<ItemTypeEntity>> GetAllItemTypesAsync()
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.ItemTypes.GetAllAsync();
        }
    }
}