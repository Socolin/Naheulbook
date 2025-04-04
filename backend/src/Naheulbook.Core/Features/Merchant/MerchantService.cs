using System.Threading;
using System.Threading.Tasks;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Merchant;

public interface IMerchantService
{
    Task<MerchantEntity> CreateAsync(
        NaheulbookExecutionContext executionContext,
        int groupId,
        CreateMerchantRequest request,
        CancellationToken cancellationToken = default
    );
}

public class MerchantService(
    IUnitOfWorkFactory unitOfWorkFactory,
    INotificationSessionFactory notificationSessionFactory,
    IMerchantFactory merchantFactory,
    IAuthorizationUtil authorizationUtil
) : IMerchantService
{
    public async Task<MerchantEntity> CreateAsync(
        NaheulbookExecutionContext executionContext,
        int groupId,
        CreateMerchantRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var group = await uow.Groups.GetAsync(groupId);
        authorizationUtil.EnsureIsGroupOwner(executionContext, group);
        if (group is null)
            throw new GroupNotFoundException(groupId);

        var notificationSession = notificationSessionFactory.CreateSession();
        var merchant = merchantFactory.Create(groupId, request);
        uow.Merchants.Add(merchant);
        notificationSession.NotifyGroupAddMerchant(groupId, merchant);

        await uow.SaveChangesAsync();

        await notificationSession.CommitAsync();

        return merchant;
    }
}