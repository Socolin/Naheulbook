using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ISlotRepository : IRepository<SlotEntity>
    {
    }

    public class SlotRepository : Repository<SlotEntity, NaheulbookDbContext>, ISlotRepository
    {
        public SlotRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}