using AutoMapper;

namespace Naheulbook.Data.Tests.Integration.EntityBuilders
{
    public class BuilderBase<TEntity> where TEntity : new()
    {
        protected readonly TEntity Entity = new TEntity();

        public TEntity Build()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TEntity, TEntity>());
            return config.CreateMapper().Map<TEntity, TEntity>(Entity);
        }
    }
}