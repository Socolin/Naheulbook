using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Data.Factories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}