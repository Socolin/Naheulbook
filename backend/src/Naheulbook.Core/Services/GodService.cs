using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface IGodService
{
    Task<List<GodEntity>> GetAllGodsAsync();
}

public class GodService(IUnitOfWorkFactory unitOfWorkFactory) : IGodService
{
    public async Task<List<GodEntity>> GetAllGodsAsync()
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.Gods.GetAllAsync();
        }
    }
}