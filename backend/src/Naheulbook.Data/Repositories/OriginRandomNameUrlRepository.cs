#pragma warning disable 8619
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IOriginRandomNameUrlRepository : IRepository<OriginRandomNameUrlEntity>
{
    Task<OriginRandomNameUrlEntity?> GetByOriginIdAndSexAsync(string sex, Guid originId);
}

public class OriginRandomNameUrlRepository : Repository<OriginRandomNameUrlEntity, NaheulbookDbContext>, IOriginRandomNameUrlRepository
{
    public OriginRandomNameUrlRepository(NaheulbookDbContext context) : base(context)
    {
    }

    public Task<OriginRandomNameUrlEntity?> GetByOriginIdAndSexAsync(string sex, Guid originId)
    {
        return Context.OriginRandomNameUrls
            .SingleOrDefaultAsync(x => x.Sex == sex && x.OriginId == originId);
    }
}