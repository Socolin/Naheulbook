using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Item;

public interface IItemTypeService
{
    Task<List<ItemTypeEntity>> GetAllItemTypesAsync(CancellationToken cancellationToken = default);
}

public class ItemTypeService(IUnitOfWorkFactory unitOfWorkFactory) : IItemTypeService
{
    public async Task<List<ItemTypeEntity>> GetAllItemTypesAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.ItemTypes.GetAllAsync(cancellationToken);
    }
}