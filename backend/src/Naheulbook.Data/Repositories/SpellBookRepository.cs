using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ISpellBookRepository : IRepository<SpellBookEntity>
{
    Task<List<SpellBookEntity>> GetAllWithSpellsAsync(
        CancellationToken cancellationToken = default
    );
}

public class SpellBookRepository(NaheulbookDbContext context)
    : Repository<SpellBookEntity, NaheulbookDbContext>(context), ISpellBookRepository
{
    public async Task<List<SpellBookEntity>> GetAllWithSpellsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.SpellBooks
            .Include(x => x.Spells.OrderBy(s => s.DisplayOrder))
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
