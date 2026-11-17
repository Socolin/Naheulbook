namespace Naheulbook.Data.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    ValueTask<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default);
    ValueTask<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}