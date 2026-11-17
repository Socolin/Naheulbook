using AwesomeAssertions;
using Naheulbook.Core.Features.Merchant;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Merchant;

[TestFixture]
[TestOf(typeof(MerchantMapper))]
public class MerchantMapperTests
{
    private MerchantMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new MerchantMapper();
    }

    [Test]
    public void ToResponse_Merchant_MapsIdAndName()
    {
        var merchant = new MerchantEntity
        {
            Id = 3,
            Name = "some-name",
            GroupId = 8,
        };

        var actual = _mapper.ToResponse(merchant);

        actual.Should().BeEquivalentTo(new MerchantResponse
        {
            Id = 3,
            Name = "some-name",
        });
    }
}
