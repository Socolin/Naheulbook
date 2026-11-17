using AwesomeAssertions;
using NSubstitute;
using NUnit.Framework;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Origin;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Controllers;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class OriginsControllerTests
{
    private IOriginService _originService;
    private IOriginMapper _originMapper;
    private ICharacterRandomNameService _characterRandomNameService;

    private OriginsController _originsController;

    [SetUp]
    public void SetUp()
    {
        _originService = Substitute.For<IOriginService>();
        _originMapper = Substitute.For<IOriginMapper>();
        _characterRandomNameService = Substitute.For<ICharacterRandomNameService>();

        _originsController = new OriginsController(_originService, _originMapper, _characterRandomNameService);
    }

    [Test]
    public async Task CanGetOrigins()
    {
        var entity1 = new OriginEntity();
        var entity2 = new OriginEntity();
        var origins = new List<OriginEntity> {entity1, entity2};
        var response1 = new OriginResponse();
        var response2 = new OriginResponse();

        _originService.GetOriginsAsync()
            .Returns(origins);
        _originMapper.ToResponse(entity1)
            .Returns(response1);
        _originMapper.ToResponse(entity2)
            .Returns(response2);

        var result = await _originsController.GetAsync();

        result.Value.Should().Equal(response1, response2);
    }
}