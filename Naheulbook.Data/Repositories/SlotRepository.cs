using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ISlotRepository : IRepository<Slot>
    {
    }

    public class SlotRepository : Repository<Slot, NaheulbookDbContext>, ISlotRepository
    {
        public SlotRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}