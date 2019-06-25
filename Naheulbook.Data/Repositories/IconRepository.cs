using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IIconRepository : IRepository<Icon>
    {
    }

    public class IconRepository : Repository<Icon, NaheulbookDbContext>, IIconRepository
    {
        public IconRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}