using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ISlotRepository : IRepository<SlotEntity>;

public class SlotRepository(NaheulbookDbContext context) : Repository<SlotEntity, NaheulbookDbContext>(context), ISlotRepository;