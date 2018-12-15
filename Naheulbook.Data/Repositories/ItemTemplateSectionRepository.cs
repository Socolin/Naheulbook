using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateSectionRepository : IRepository<ItemTemplateSection>
    {
    }

    public class ItemTemplateSectionRepository : Repository<ItemTemplateSection, NaheulbookDbContext>, IItemTemplateSectionRepository
    {
        public ItemTemplateSectionRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}