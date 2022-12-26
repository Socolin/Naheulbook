using Naheulbook.Data.Extensions.UnitOfWorks;

namespace Naheulbook.Data.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}