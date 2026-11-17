using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Merchant;

public interface IMerchantMapper
{
    MerchantResponse ToResponse(MerchantEntity merchant);
}

public class MerchantMapper : IMerchantMapper
{
    public MerchantResponse ToResponse(MerchantEntity merchant)
    {
        return new MerchantResponse
        {
            Id = merchant.Id,
            Name = merchant.Name,
        };
    }
}
