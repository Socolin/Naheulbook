using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naheulbook.Data.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    ValueTask<TEntity?> GetAsync(int id);
    ValueTask<TEntity?> GetAsync(Guid id);
    Task<List<TEntity>> GetAllAsync();

    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}