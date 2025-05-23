using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterSpecialityRepository : IRepository<CharacterSpecialityEntity>
{
    Task<List<SpecialityEntity>> GetWithModiferWithSpecialByIdsAsync(List<Guid> specialityIds);
}

public class CharacterSpecialityRepository(NaheulbookDbContext naheulbookDbContext) : Repository<CharacterSpecialityEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterSpecialityRepository
{
    public Task<List<SpecialityEntity>> GetWithModiferWithSpecialByIdsAsync(List<Guid> specialityIds)
    {
        return Context.Specialities
            .Include(s => s.Modifiers)
            .Include(s => s.Specials)
            .Where(s => specialityIds.Contains(s.Id))
            .ToListAsync();
    }
}