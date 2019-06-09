using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateRepository : IRepository<ItemTemplate>
    {
        Task <List<ItemTemplate>> GetByIdsAsync(IEnumerable<int> ids);
    }

    public class ItemTemplateRepository : Repository<ItemTemplate, NaheulbookDbContext>, IItemTemplateRepository
    {
        public ItemTemplateRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<ItemTemplate>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return Context.ItemTemplates
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }
    }
}