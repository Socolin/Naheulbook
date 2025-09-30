using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Aptitude;

public interface IAptitudeGroupService
{
    Task<List<AptitudeGroupEntity>> GetAptitudeGroupListAsync(
        CancellationToken cancellationToken = default
    );

    Task<AptitudeGroupEntity> GetAptitudeGroupAsync(
        Guid aptitudeGroupId,
        CancellationToken cancellationToken = default
    );
}

public class AptitudeGroupService(
    IUnitOfWorkFactory unitOfWorkFactory
) : IAptitudeGroupService
{
    public async Task<List<AptitudeGroupEntity>> GetAptitudeGroupListAsync(
        CancellationToken cancellationToken
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.AptitudeGroupRepository.GetAllAsync();
    }

    public async Task<AptitudeGroupEntity> GetAptitudeGroupAsync(
        Guid aptitudeGroupId,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var aptitudeGroup = await uow.AptitudeGroupRepository.GetByIdWithAptitudesAsync(aptitudeGroupId, cancellationToken);
        if (aptitudeGroup == null)
            throw new AptitudeGroupNotFoundException(aptitudeGroupId);

        return aptitudeGroup;
    }
}