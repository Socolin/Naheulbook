using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Spell;

public interface ISpellService
{
    Task<List<SpellBookEntity>> GetSpellBooksAsync(CancellationToken cancellationToken = default);
}

public class SpellService(IUnitOfWorkFactory unitOfWorkFactory) : ISpellService
{
    public async Task<List<SpellBookEntity>> GetSpellBooksAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.SpellBooks.GetAllWithSpellsAsync(cancellationToken);
    }
}
