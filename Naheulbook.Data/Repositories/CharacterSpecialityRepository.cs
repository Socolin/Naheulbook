using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICharacterSpecialityRepository : IRepository<CharacterSpeciality>
    {
        Task<List<Speciality>> GetWithModiferWithSpecialByIdsAsync(List<Guid> specialityIds);
    }

    public class CharacterSpecialityRepository : Repository<CharacterSpeciality, NaheulbookDbContext>, ICharacterSpecialityRepository
    {
        public CharacterSpecialityRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }

        public Task<List<Speciality>> GetWithModiferWithSpecialByIdsAsync(List<Guid> specialityIds)
        {
            return Context.Specialities
                .Include(s => s.Modifiers)
                .Include(s => s.Specials)
                .Where(s => specialityIds.Contains(s.Id))
                .ToListAsync();
        }
    }
}