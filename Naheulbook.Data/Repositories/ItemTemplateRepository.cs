using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateRepository : IRepository<ItemTemplate>
    {
    }

    public class ItemTemplateRepository : Repository<ItemTemplate, NaheulbookDbContext>, IItemTemplateRepository
    {
        public ItemTemplateRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}