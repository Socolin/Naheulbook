using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Item;

public interface IItemTypeService
{
    Task<List<ItemTypeEntity>> GetAllItemTypesAsync();
}

public class ItemTypeService(IUnitOfWorkFactory unitOfWorkFactory) : IItemTypeService
{
    public async Task<List<ItemTypeEntity>> GetAllItemTypesAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.ItemTypes.GetAllAsync();
    }
}