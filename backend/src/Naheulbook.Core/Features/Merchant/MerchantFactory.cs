using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Merchant;

public interface IMerchantFactory
{
    MerchantEntity Create(int groupId, CreateMerchantRequest request);
}

public class MerchantFactory : IMerchantFactory
{
    public MerchantEntity Create(int groupId, CreateMerchantRequest request)
    {
        return new MerchantEntity
        {
            GroupId = groupId,
            Name = request.Name,
        };
    }
}