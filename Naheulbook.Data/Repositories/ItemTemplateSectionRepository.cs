using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateSectionRepository : IRepository<ItemTemplateSection>
    {
        Task<List<ItemTemplateSection>> GetAllWithCategoriesAsync();
    }

    public class ItemTemplateSectionRepository : Repository<ItemTemplateSection, NaheulbookDbContext>, IItemTemplateSectionRepository
    {
        public ItemTemplateSectionRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<ItemTemplateSection>> GetAllWithCategoriesAsync()
        {
            return Context.ItemTemplateSections
                .Include(s => s.Categories)
                .ToListAsync();
        }
    }
}