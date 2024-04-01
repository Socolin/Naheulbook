using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ISlotRepository : IRepository<SlotEntity>;

public class SlotRepository(NaheulbookDbContext context) : Repository<SlotEntity, NaheulbookDbContext>(context), ISlotRepository;