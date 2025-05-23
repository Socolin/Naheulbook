#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;

namespace Naheulbook.Data.Repositories;

public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class where TContext : DbContext
{
    protected readonly TContext Context;

    protected Repository(TContext context)
    {
        Context = context;
    }

    public ValueTask<TEntity?> GetAsync(int id)
    {
        return Context.Set<TEntity>().FindAsync(id);
    }

    public ValueTask<TEntity?> GetAsync(Guid id)
    {
        return Context.Set<TEntity>().FindAsync(id);
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        return Context.Set<TEntity>().ToListAsync();
    }

    public void Add(TEntity entity)
    {
        Context.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        Context.AddRange(entities);
    }

    public void Remove(TEntity entity)
    {
        Context.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        Context.RemoveRange(entities);
    }
}